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
        
            List<Message> newsItems = InfowijsMessages.GetBetterInfowijsMessages() ?? new List<Message>();

<<<<<<< HEAD
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
            var messages = new CoroutineWithData<List<Message>>(this, InfowijsMessages.GetBetterInfowijsMessages()).result;
            foreach (Message message in messages)
=======
            foreach (Message message in newsItems?.OrderByDescending(x => x.createdAt)!)
>>>>>>> parent of 89cbb2a... Just saving 33% on loading times. nothing to much ;)
            {
                GameObject newsItem = Instantiate(_newsItemPrefab, _newsItemContainer.transform);
                newsItem.GetComponent<SchoolNews>().Initialize(message);
            }
        
            base.Initialize();
        }
    }
}
