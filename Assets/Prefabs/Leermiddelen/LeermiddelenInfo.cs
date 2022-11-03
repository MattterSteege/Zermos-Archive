using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeermiddelenInfo : MonoBehaviour
{
    [SerializeField] TMP_Text leermiddelenText;
    [SerializeField] Button leermiddelenButton;
    
    public void SetLeermiddelenText(string vak, string link)
    {
        leermiddelenText.text = $"<link=\"{link}\">{vak}</link>";
        
        leermiddelenButton.onClick.AddListener(() => Application.OpenURL(link));
    }
}
