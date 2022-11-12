using System.Collections;
using System.Collections.Generic;
using UI.Views;
using UnityEngine;

public class SchoolNewsView : View
{
    [SerializeField] private GameObject _newsItemPrefab;
    [SerializeField] private GameObject _newsItemContainer;
    [SerializeField] private Messages InfowijsMessages;
    
    public override void Initialize()
    {
        List<Message> newsItems = InfowijsMessages.GetBetterInfowijsMessages();

        foreach (Message message in newsItems)
        {
            GameObject newsItem = Instantiate(_newsItemPrefab, _newsItemContainer.transform);
            newsItem.GetComponent<SchoolNews>().Initialize(message);
        }
        
        base.Initialize();
    }
}
