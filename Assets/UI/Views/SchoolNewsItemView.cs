using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Views;
using UnityEngine;

public class SchoolNewsItemView : View
{
    [SerializeField] private TMP_Text messageText;
    
    public override void Show(object args = null)
    {
        openNavigationButton.onClick.RemoveAllListeners();
        openNavigationButton.onClick.AddListener(() => ViewManager.Instance.ShowNewView<SchoolNewsView>());
        
        var message = (Message) args;
        messageText.text = message.content ?? "Fetching error";
        
        base.Show(args);
    }
    
    public override void Refresh(object args)
    {
        openNavigationButton.onClick.RemoveAllListeners();
        base.Refresh(args);
    }
}
