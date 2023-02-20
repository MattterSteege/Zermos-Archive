using System;
using System.Collections;
using DG.Tweening;
using Michsky.MUIP;
using TMPro;
using UnityEngine;

namespace UI.Views
{
    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] TMP_Text loadingText;
        [SerializeField] TMP_Text versionText;
        [SerializeField] GameObject loadingScreen;
        [SerializeField] ProgressBar progressBar;
        [SerializeField] OpenMenu openMenu;

        void Awake()
        {
            gameObject.SetActive(true);
            loadingScreen.GetComponent<CanvasGroup>().alpha = 1;
        
            versionText.text = Application.version;
            
            ViewManager.onLoadedView += OnLoadedView;
            ViewManager.onInitializeComplete += Complete;
        }

        private void OnLoadedView(float loadingcomplete, string viewName)
        {
            loadingScreen.SetActive(true);
            loadingText.text = "Laden: " + loadingcomplete.ToString("P0");
            progressBar.ChangeValue(loadingcomplete * 100f);
            //loadingInfoText.text = viewName;
        
            if (Math.Abs(loadingcomplete - 1) < 0.05f)
            {
                ViewManager.onLoadedView -= OnLoadedView;
                ViewManager.onInitializeComplete -= Complete;
                Complete(true);
            }
        }

        private void Complete(bool done)
        {
            progressBar.currentPercent = 100;
            loadingScreen.GetComponent<CanvasGroup>().DOFade(1, 0.5f).onComplete += () =>
            {
                loadingScreen.GetComponent<CanvasGroup>().blocksRaycasts = false;
                openMenu._CloseMenu();
            };
        }
    }
}
