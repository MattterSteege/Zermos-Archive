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
    public List<Messages.Message> messages;
    public SubViewManager subViewManager;

    public void Initialize()
    {
        GetComponent<Button>().onClick.AddListener(() =>  subViewManager.ShowNewView<SchoolNewsItemView>(messages));
    }
}
