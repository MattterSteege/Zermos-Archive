using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class SchoolNewsView : View
    {
        [SerializeField] SubViewManager subViewManager;
        [SerializeField] private GameObject _newsItemPrefab;
        [SerializeField] private GameObject _newsItemContainer;
        [SerializeField] private Messages InfowijsMessages;
        [SerializeField] private bool loaded = false;
        [SerializeField] private int messagesToLoad = 25;
        [SerializeField] GameObject DividerPrefab;
        [SerializeField] GameObject dayContainterPrefab;

        public override void Initialize()
        {
            MonoBehaviour Mono = ViewManager.Instance.GetComponent<MonoBehaviour>();
            Mono.StartCoroutine(subViewManager.Initialize());
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
            var messages = new CoroutineWithData<Messages.InfowijsMessage>(this, InfowijsMessages.getMessages()).result;
            messages.Data.Messages = messages.Data.Messages.Take(messagesToLoad).ToList();
            yield return null;
            SchoolNews CurrentItem = null; 
            int CurrentItemType = 0;
            int day = -1;
            GameObject dayContainer = null;
            
            foreach (Messages.Message message in messages.Data.Messages)
            {
                if (message.CreatedAt.ToDateTime().Day != day)
                {
                    dayContainer = Instantiate(dayContainterPrefab, _newsItemContainer.transform);
#if UNITY_EDITOR
                    dayContainer.name = "DayContainer " + message.CreatedAt.ToDateTime().ToString("d MMMM");
#endif
                    var go = Instantiate(DividerPrefab, dayContainer.transform);
                    go.GetComponent<homeworkDivider>().Datum.text = message.CreatedAt.ToDateTime().ToString("d MMMM");
                    day = message.CreatedAt.ToDateTime().Day;
                    
                }

                if (CurrentItemType < message.Type)
                {
                    var go = Instantiate(_newsItemPrefab, dayContainer.transform);
                    CurrentItem = go.GetComponent<SchoolNews>();
                    
                    if (CurrentItem != null)
                        CurrentItem.Initialize();
                    CurrentItem.messages = new List<Messages.Message>();
                    CurrentItem.subViewManager = subViewManager;

                }

                if (message.Type == 30)
                {
                    CurrentItem.titleText.text = message.Content.ContentClass.Title;
                    CurrentItem.dateText.text = message.CreatedAt.ToDateTime().ToString("d MMMM");
                }
                
                if (message.Type == 1)
                {
                    CurrentItem.messageText.text = message.Content.String;
                }
                
                CurrentItem.messages.Add(message);
                CurrentItemType = (int) message.Type;
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