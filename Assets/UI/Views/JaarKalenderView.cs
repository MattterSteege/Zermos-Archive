using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class JaarKalenderView : View
    {
        [SerializeField] private GameObject _kalenderItemPrefab;
        [SerializeField] private GameObject _kalenderItemContainer;
        [SerializeField] private JaarKalender InfowijsJaarKalender;
        [SerializeField] private TMP_Text beschrijving;
        private bool loaded = false;
        [SerializeField] private GameObject CurrentDateDiver;
        [SerializeField] private ScrollRect _ScrollRect;
    
        public override void Initialize()
        {
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

            base.Initialize();
        }

        public override void Show(object args = null)
        {
            base.Show(args);
            
            if (!loaded)
            {
                loaded = true;
                StartCoroutine(PopulateKalenderItems());
            }
            else
            {
                MonoBehaviour camMono = ViewManager.Instance.GetComponent<MonoBehaviour>();
                camMono.StartCoroutine(ShowKalenderItems());
            }
        }
        
        public override void Hide()
        {
            MonoBehaviour camMono = ViewManager.Instance.GetComponent<MonoBehaviour>();
            camMono.StartCoroutine(HideKalenderItems());

            base.Hide();
        }
        
        private IEnumerator ShowKalenderItems()
        {
            foreach (Transform child in _kalenderItemContainer.transform) 
            {
                child.gameObject.SetActive(true);
                yield return new WaitForEndOfFrame();
            }
        }
        
        private IEnumerator HideKalenderItems()
        {
            foreach (Transform child in _kalenderItemContainer.transform)
            {
                child.gameObject.SetActive(false);
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator PopulateKalenderItems()
        {
            yield return new WaitForSeconds(0.5f);
            var messages = new CoroutineWithData<JaarKalender.InfowijsKalender>(this, InfowijsJaarKalender.DownloadMessages()).result;
            beschrijving.DOFade(0f, 0.1f);
            yield return new WaitForSeconds(0.1f);
            foreach (JaarKalender.Datum afspraak in messages.data)
            {
                if (afspraak.endsAt.ToDateTime() > TimeManager.Instance.DateTime && CurrentDateDiver == null)
                {
                    CurrentDateDiver = new GameObject("CurrentDate");
                    CurrentDateDiver.transform.SetParent(_kalenderItemContainer.transform);
                    //set height to 1
                    CurrentDateDiver.AddComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1);
                }
                
                var go = Instantiate(_kalenderItemPrefab, _kalenderItemContainer.transform);
                go.GetComponent<JaarKalenderPrefab>().Initialize(afspraak);
                go.GetComponent<CanvasGroup>().alpha = 0;
                go.GetComponent<CanvasGroup>().DOFade(1f, 1f);
                
            }

            yield return new WaitForSeconds(0.1f);
            
            _ScrollRect.decelerationRate = 0f;
            _ScrollRect.vertical = false;
            _ScrollRect.content.DOLocalMove(_ScrollRect.GetSnapToPositionToBringChildIntoView(CurrentDateDiver.GetComponent<RectTransform>()), 1f, true).onComplete += () => 
            {
                _ScrollRect.decelerationRate = 0.135f;
                _ScrollRect.vertical = true;
            };
        }
        
        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }
}