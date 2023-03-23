using System.Collections.Generic;
using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

public class SchoolNewsItemView : SubView
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text messageTitleText;
    [SerializeField] private TMP_Text MessageDateText;
    [SerializeField] private GameObject BijlagePrefab;
    [SerializeField] private GameObject Content;

    public override void Show(object args = null)
    {
        var message = (List<Messages.Message>) args;

        foreach (Transform child in Content.transform)
            Destroy(child.gameObject);
        
        if (message != null)
            foreach (var x in message)
            {
                if (x.Type == 1)
                    messageText.text = x.Content.String;

                if (x.Type == 30)
                {
                    messageTitleText.text = x.Content.ContentClass.Title;
                    if (x.Content.ContentClass.Timestamp != null)
                        MessageDateText.text = x.Content.ContentClass.Timestamp.Value.ToDateTime().ToString("d MMMM yyyy");
                }

                if (x.Type == 2)
                {
                    var attachment = Instantiate(BijlagePrefab, Content.transform);
                    attachment.GetComponentInChildren<TMP_Text>().text = x.Content.ContentClass.Name;
                    attachment.GetComponentInChildren<Button>().onClick.AddListener(() => Application.OpenURL(x.Content.ContentClass.Url.AbsoluteUri));
                }
            }

        base.Show(args);
    }

    public override void Initialize()
    {
        backButton.onClick.AddListener(() =>
        {
            gameObject.GetComponentInParent<SubViewManager>().ShowParentView();
        });
        
        base.Initialize();
    }

    public override void Refresh(object args)
    {
        backButton.onClick.RemoveAllListeners();
        base.Refresh(args);
    }
}
