using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

namespace UpgradeSystem {

   struct GameData
   {
      public string gameName;
      public string description;
      public string version;
      public string url;
   }

   public class NewUpdatesPopupUI : MonoBehaviour {

      [Header ( "## UI References :" )]
      [SerializeField] GameObject uiCanvas;
      [SerializeField] Button uiNotNowButton;

        [Space(20f)]
        [Header("## Settings :")]
      [SerializeField] Button uiUpdateButton;
      [SerializeField] TextMeshProUGUI uiDescriptionText;



      private string jsonDataURL = "https://www.voxelium.games/gamedev/games_data/KittyPrankster_data.json";
      static bool isAlreadyCheckedForUpdates = false;

      GameData latestGameData;

      void Start ( )

      {
         if ( !isAlreadyCheckedForUpdates )
         {
            StartCoroutine (CheckForUpdates());
         }
      }

      IEnumerator CheckForUpdates()
        {
         UnityWebRequest request = UnityWebRequest.Get (jsonDataURL);

         //request.chunkedTransfer = false;
         request.disposeDownloadHandlerOnDispose = true;
         request.timeout = 60;

         yield return request.SendWebRequest();

         if ( request.isDone )
         {
            isAlreadyCheckedForUpdates = true;

            if ( request.result != UnityWebRequest.Result.ConnectionError)
            {
               latestGameData = JsonUtility.FromJson<GameData> (request.downloadHandler.text);
               if ( !string.IsNullOrEmpty (latestGameData.version) && !Application.version.Equals (latestGameData.version))
               {
                  // new update is available
                  uiDescriptionText.text = latestGameData.description;
                  ShowPopup ( );
               }
            }
         }

         request.Dispose();
      }

      void ShowPopup ( ) {
         // Add buttons click listeners :
         uiNotNowButton.onClick.AddListener ( ( ) => {HidePopup ( );});

         uiUpdateButton.onClick.AddListener ( ( ) => {
            Application.OpenURL ( latestGameData.url );
            HidePopup ( );
         } );

         uiCanvas.SetActive ( true );
      }

      void HidePopup ( ) {
         uiCanvas.SetActive ( false );

         // Remove buttons click listeners :
         uiNotNowButton.onClick.RemoveAllListeners ( );
         uiUpdateButton.onClick.RemoveAllListeners ( );
      }


      void OnDestroy ()
      {
         StopCoroutine(CheckForUpdates());
      }
	   
   }

}
