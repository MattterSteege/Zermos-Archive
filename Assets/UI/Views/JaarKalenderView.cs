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
        private bool loaded = false;
        [SerializeField] private RectTransform CurrentDateDiver;
        [SerializeField] private ScrollRect _ScrollRect;
        [SerializeField] GameObject DividerPrefab;
        [SerializeField] GameObject dayContainterPrefab;
        [SerializeField] AnimationCurve _curve;


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
            Recenter();
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
            yield return null;
            var messages = new CoroutineWithData<JaarKalender.InfowijsKalender>(this, InfowijsJaarKalender.DownloadMessages()).result;
            yield return null;
            foreach (JaarKalender.Datum afspraak in messages.data)
            {
                GameObject dayContainer = Instantiate(dayContainterPrefab, _kalenderItemContainer.transform);
                
                var go = Instantiate(DividerPrefab, dayContainer.transform);
                go.GetComponent<homeworkDivider>().Datum.text = afspraak.startsAt.ToDateTime().ToString("dd MMMM") + " t/m " + afspraak.endsAt.ToDateTime().ToString("dd MMMM");
                
                go = Instantiate(_kalenderItemPrefab, dayContainer.transform);
                go.GetComponent<JaarKalenderPrefab>().Initialize(afspraak);
                
                if (afspraak.endsAt.ToDateTime() > TimeManager.Instance.DateTime && CurrentDateDiver == null)
                {
                    CurrentDateDiver = dayContainer.GetComponent<RectTransform>();
                }
            }
            
            yield return null;
            Recenter();
        }
        
        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
        
        //context menu recenter _> _ScrollRect.decelerationRate = 0f;
        //_ScrollRect.vertical = false;
        //_ScrollRect.ScrollToTop(CurrentDateDiver, _curve);
        
        [ContextMenu("Recenter")]
        public void Recenter()
        {
            _ScrollRect.decelerationRate = 0f;
            _ScrollRect.vertical = false;
            _ScrollRect.ScrollToTop(CurrentDateDiver, _curve);
        }
    }
}