using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SchoolNews : MonoBehaviour
{
    [SerializeField] public TMP_Text titleText;
    [SerializeField] public TMP_Text messageText;
    [SerializeField] public TMP_Text dateText;
    [SerializeField] public Button OpenMessageButton;
    [SerializeField] public List<Messages.Message> messages;
    
    public void Initialize()
    {
        OpenMessageButton.onClick.AddListener(() => ViewManager.Instance.ShowNewView<SchoolNewsItemView>(messages));
    }
}
