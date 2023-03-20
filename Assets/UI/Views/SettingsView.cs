using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.SubViews;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class SettingsView : View
    {
        [SerializeField] private SubViewManager subViewManager;
        
        [Header("Secret Settings")]
        [SerializeField] private Button SecretSettingsButton;
#if !UNITY_EDITOR
        private int ClicksNeeded = 5;
#endif
        [Header("SubView buttons")]
        [SerializeField] private Button algemeneInstellingenButton;
        [SerializeField] private Button koppelingenButton;
        [SerializeField] private Button userInfoButton;
        [SerializeField] private Button hulpNodigButton;
        [Header("Snelle links")]
        [SerializeField] private Button zermeloButton;
        [SerializeField] private Button somtodayButton;
        [SerializeField] private Button OfficeButton;
        [SerializeField] private Button schoolwikiButton;
        
        [SerializeField] RectTransform SourceRectTransform;

        public override void Initialize()
        {
            MonoBehaviour Mono = ViewManager.Instance.GetComponent<MonoBehaviour>();
            Mono.StartCoroutine(subViewManager.Initialize());

#if !UNITY_EDITOR
            int timesCLicked = 0;

            SecretSettingsButton.onClick.AddListener(() =>
            {
                timesCLicked++;
                if (timesCLicked >= ClicksNeeded)
                {
                    timesCLicked = 0;
                    AndroidUIToast.ShowToast("WEES VOORZICHTIG!");
                    subViewManager.ShowNewView<SecretSettingsView>();
                }
            });
#else
            SecretSettingsButton.onClick.AddListener(() =>
            {
                subViewManager.ShowNewView<SecretSettingsView>();
            });
#endif
            
            #region SubView buttons
            
            algemeneInstellingenButton.onClick.AddListener(() =>
            {
                subViewManager.ShowNewView<AlgemeneInstellingenSubView>();
            });
            
            koppelingenButton.onClick.AddListener(() =>
            {
                subViewManager.ShowNewView<KoppelingenSubView>();
            });
            
            userInfoButton.onClick.AddListener(() =>
            {
                subViewManager.ShowNewView<SavedUserInformationSubView>();
            });

            hulpNodigButton.onClick.AddListener(() => Application.OpenURL(@"https://mjtsgamer.github.io/Zermos/"));
            // zermeloButton.onClick.AddListener(() => Application.OpenURL(@"https://ccg.zportal.nl/"));
            // somtodayButton.onClick.AddListener(() => Application.OpenURL(@"https://carmelcollegegouda.somtoday.nl/"));
            // OfficeButton.onClick.AddListener(() => Application.OpenURL(@"https://portal.office.com/"));
            // schoolwikiButton.onClick.AddListener(() => Application.OpenURL(@"https://antonius.schoolwiki.nl/antoniuscollege"));
            #endregion
        }

        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            SecretSettingsButton.onClick.RemoveAllListeners();
            algemeneInstellingenButton.onClick.RemoveAllListeners();
            koppelingenButton.onClick.RemoveAllListeners();
            // userInfoButton.onClick.RemoveAllListeners();
            hulpNodigButton.onClick.RemoveAllListeners();
            zermeloButton.onClick.RemoveAllListeners();
            somtodayButton.onClick.RemoveAllListeners();
            OfficeButton.onClick.RemoveAllListeners();
            schoolwikiButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }
}