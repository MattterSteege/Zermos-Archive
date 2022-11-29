using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.Views
{
    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] TMP_Text loadingText;
        [SerializeField] TMP_Text versionText;
        [SerializeField] UpdateSystem updateSystem;
        [SerializeField] GameObject loadingScreen;

        void Awake()
        {
            gameObject.SetActive(true);
            loadingScreen.GetComponent<CanvasGroup>().alpha = 1;
        
            versionText.text = updateSystem.CurrentVersion;
            
            ViewManager.onLoadedView += OnLoadedView;
            ViewManager.onInitializeComplete += Complete;
        }

        private void OnLoadedView(float loadingcomplete)
        {
            loadingScreen.SetActive(true);
            loadingText.text = "Laden: " + loadingcomplete.ToString("P0");
        
            if (Math.Abs(loadingcomplete - 1) < 0.05f)
            {
                ViewManager.onLoadedView -= OnLoadedView;
                ViewManager.onInitializeComplete -= Complete;
                Complete(true);
            }
        }

        private void Complete(bool done)
        {
            loadingScreen.GetComponent<CanvasGroup>().DOFade(1, 0.5f).onComplete += () =>
            {
                loadingScreen.GetComponent<CanvasGroup>().blocksRaycasts = false;
                loadingScreen.GetComponent<CanvasGroup>().DOFade(0, 1.5f).onComplete += () =>
                {
                    gameObject.SetActive(false);
                    Destroy(gameObject);
                };
            };
        }
    }
}
