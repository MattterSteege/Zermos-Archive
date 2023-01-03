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
        [SerializeField] private Button HulpNodigButton;

        public override void Initialize()
        {
            MonoBehaviour camMono = ViewManager.Instance.GetComponent<MonoBehaviour>();
            camMono.StartCoroutine(subViewManager.Initialize());
            
            openNavigationButton.onClick.AddListener(() =>
            {
                openNavigationButton.enabled = false;
                ViewManager.Instance.ShowNavigation();
            });

            closeButtonWholePage.onClick.AddListener(() =>
            {
                openNavigationButton.enabled = true;
                ViewManager.Instance.HideNavigation();
            });
            
#if !UNITY_EDITOR
            int timesCLicked = 0;

            SecretSettingsButton.onClick.AddListener(() =>
            {
            timesCLicked++;
            if (timesCLicked >= ClicksNeeded)
                {
                    timesCLicked = 0;
                    ViewManager.Instance.ShowNewView<SecretSettingsView>();
                }
            });
#else
            SecretSettingsButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<SecretSettingsView>();
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
            
            // userInfoButton.onClick.AddListener(() =>
            // {
            //     subViewManager.ShowNewView<UserInfoSubView>();
            // });

            HulpNodigButton.onClick.AddListener(() =>
            {
                Application.OpenURL(@"https://mjtsgamer.github.io/Zermos/");
            });
            
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
            HulpNodigButton.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }
}