using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SchoolNews : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private Button OpenMessageButton;
    
    public void Initialize(Message message)
    {
        messageText.text = message.content;
        dateText.text = message.createdAt.ToDateTime().ToString("d MMMM yyyy");
        OpenMessageButton.onClick.AddListener(() => ViewManager.Instance.ShowNewView<SchoolNewsItemView>(message));
    }
}
