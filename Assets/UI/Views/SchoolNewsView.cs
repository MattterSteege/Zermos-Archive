using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.Views
{
    public class SchoolNewsView : View
    {
        [SerializeField] private GameObject _newsItemPrefab;
        [SerializeField] private GameObject _newsItemContainer;
        [SerializeField] private Messages InfowijsMessages;
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
        }

        private IEnumerator PopulateNewsItems()
        {
            yield return new WaitForSeconds(0.5f);
            var messages = new CoroutineWithData<List<Message>>(this, InfowijsMessages.GetBetterInfowijsMessages()).result;
            foreach (Message message in messages)
            {
                GameObject newsItem = Instantiate(_newsItemPrefab, _newsItemContainer.transform);
                newsItem.GetComponent<SchoolNews>().Initialize(message);
                yield return new WaitForEndOfFrame();
            }
        }
    }   
}
