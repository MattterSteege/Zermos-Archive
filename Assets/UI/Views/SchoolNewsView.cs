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
        [SerializeField] private int messagesToLoad = 25;
    
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
            var messages = new CoroutineWithData<Messages.InfowijsMessage>(this, InfowijsMessages.getMessages()).result;
            beschrijving.DOFade(0f, 0.1f);
            yield return new WaitForSeconds(0.1f);
            SchoolNews CurrentItem = null; 
            int CurrentItemType = 0;
            // foreach (Messages.Message message in messages.Data.Messages)
            // {
            //     if (CurrentItemType < message.Type)
            //     {
            //         if (CurrentItem != null)
            //             CurrentItem.Initialize();
            //         CurrentItemType = 30;
            //         var go = Instantiate(_newsItemPrefab, _newsItemContainer.transform);
            //         CurrentItem = go.GetComponent<SchoolNews>();
            //         go.GetComponent<CanvasGroup>().alpha = 0;
            //         go.GetComponent<CanvasGroup>().DOFade(1f, 1f);
            //         CurrentItem.messages = new List<Messages.Message>();
            //     }
            //
            //     if (message.Type == 30)
            //     {
            //         CurrentItem.titleText.text = message.Content.ContentClass.Title;
            //         CurrentItem.dateText.text = message.CreatedAt.ToDateTime().ToString("d MMMM");
            //     }
            //     
            //     if (message.Type == 1)
            //     {
            //         CurrentItem.messageText.text = message.Content.String;
            //     }
            //     
            //     CurrentItem.messages.Add(message);
            //     CurrentItemType = (int) message.Type;
            //     yield return new WaitForEndOfFrame();
            // }
            //
            // //do this in a for loop where i > 100


            int i = 0;
            for (int x = 0; x < messagesToLoad; i++)
            {
                Messages.Message message = messages.Data.Messages[i];
                
                if (CurrentItemType < message.Type)
                {
                    if (CurrentItem != null)
                        CurrentItem.Initialize();
                    CurrentItemType = 30;
                    var go = Instantiate(_newsItemPrefab, _newsItemContainer.transform);
                    CurrentItem = go.GetComponent<SchoolNews>();
                    go.GetComponent<CanvasGroup>().alpha = 0;
                    go.GetComponent<CanvasGroup>().DOFade(1f, 1f);
                    CurrentItem.messages = new List<Messages.Message>();
                    x++;
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