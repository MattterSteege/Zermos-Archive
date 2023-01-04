using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI.Views
{
    public class SchoolNewsView : View
    {
        [SerializeField] private GameObject _newsItemPrefab;
        [SerializeField] private GameObject _newsItemContainer;
        [SerializeField] private Messages InfowijsMessages;
        [SerializeField] private TMP_Text beschrijving;
        [SerializeField] private bool loaded = false;
    
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
                StartCoroutine(PopulateNewsItems());
            }
            else
            {
                MonoBehaviour camMono = ViewManager.Instance.GetComponent<MonoBehaviour>();
                camMono.StartCoroutine(ShowNewsItems());
            }
        }
        
        public override void Hide()
        {
            MonoBehaviour camMono = ViewManager.Instance.GetComponent<MonoBehaviour>();
            camMono.StartCoroutine(HideNewsItems());

            base.Hide();
        }
        
        private IEnumerator ShowNewsItems()
        {
            foreach (Transform child in _newsItemContainer.transform) 
            {
                child.gameObject.SetActive(true);
                yield return new WaitForEndOfFrame();
            }
        }
        
        private IEnumerator HideNewsItems()
        {
            foreach (Transform child in _newsItemContainer.transform)
            {
                child.gameObject.SetActive(false);
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator PopulateNewsItems()
        {
            yield return new WaitForSeconds(0.5f);
            beschrijving.DOFade(0f, 0.1f);
            yield return new WaitForSeconds(0.05f);
            var messages = new CoroutineWithData<List<Message>>(this, InfowijsMessages.GetBetterInfowijsMessages()).result;
            foreach (Message message in messages)
            {
                GameObject newsItem = Instantiate(_newsItemPrefab, _newsItemContainer.transform);
                newsItem.GetComponent<SchoolNews>().Initialize(message);
                yield return new WaitForEndOfFrame();
            }
        }
        
        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }   
}