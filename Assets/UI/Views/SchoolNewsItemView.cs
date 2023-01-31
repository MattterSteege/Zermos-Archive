using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI.Views;
using UnityEngine;

public class SchoolNewsItemView : View
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text messageTitleText;
    [SerializeField] public List<Messages.Message> messages;
    
    public override void Show(object args = null)
    {
        openNavigationButton.onClick.RemoveAllListeners();
        openNavigationButton.onClick.AddListener(() => ViewManager.Instance.ShowNewView<SchoolNewsView>());
        
        var message = (List<Messages.Message>) args;

        Messages.Message first = null;
        if (message != null)
            foreach (var x in message)
            {
                if (x.Type == 1)
                    messageText.text = x.Content.String;

                if (x.Type == 30)
                    messageTitleText.text = x.Content.ContentClass.Title;
                
            }

        base.Show(args);
    }
    
    public override void Refresh(object args)
    {
        openNavigationButton.onClick.RemoveAllListeners();
        base.Refresh(args);
    }
}
