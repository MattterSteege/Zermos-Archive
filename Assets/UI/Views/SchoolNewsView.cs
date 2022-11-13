using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Views;
using UnityEngine;

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
        
        List<Message> newsItems = InfowijsMessages.GetBetterInfowijsMessages();

        foreach (Message message in newsItems.OrderByDescending(x => x.createdAt))
        {
            GameObject newsItem = Instantiate(_newsItemPrefab, _newsItemContainer.transform);
            newsItem.GetComponent<SchoolNews>().Initialize(message);
        }
        
        base.Initialize();
    }
}
