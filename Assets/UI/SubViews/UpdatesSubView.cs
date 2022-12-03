using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class UpdatesSubView : SubView
    {
        [SerializeField] UpdateSystem updateSystem;
        [SerializeField] Button updateButton;
        [SerializeField] TMP_Text releaseNotesText;
        
        public override void Initialize()
        {
            bool shouldUpdate = updateSystem.checkForUpdates() == 1;
            
#if UNITY_ANDROID
            if (shouldUpdate)
            {
                updateButton.GetComponentInChildren<TMP_Text>().text = "Update gevonden! downloaden?";
                updateButton.onClick.RemoveAllListeners();
                updateButton.onClick.AddListener(() =>
                {
                    updateSystem.DownloadLatestVersion();
                    
                    updateButton.GetComponentInChildren<TMP_Text>().text = "Klaar!";
                });
                
                releaseNotesText.transform.parent.gameObject.SetActive(true);
                releaseNotesText.text = updateSystem.GetLatestVersion(UpdateSystem.fetchType.release_notes);
            }
            else
            {
                updateButton.GetComponentInChildren<TMP_Text>().text = "Geen update gevonden";
                releaseNotesText.transform.parent.gameObject.SetActive(false);
            }
#elif UNITY_IOS
            searchForUpdates.GetComponentInChildren<TMP_Text>().text = "Updaten kan alleen op Android";
#endif
            
            base.Initialize();
        }
    }
}