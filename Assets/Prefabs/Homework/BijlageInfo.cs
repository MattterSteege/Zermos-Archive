using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BijlageInfo : MonoBehaviour
{
    [SerializeField] private Button openLinkButton;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    
    public void AddLink(string link)
    {
        openLinkButton.onClick.AddListener(() => Application.OpenURL(link));
    }
    
    public void setInfo(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
    }
}
