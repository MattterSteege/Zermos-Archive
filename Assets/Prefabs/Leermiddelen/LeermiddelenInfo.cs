using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeermiddelenInfo : MonoBehaviour
{
    [SerializeField] TMP_Text leermiddelenText;
    
    public void SetLeermiddelenText(string vak, string link)
    {
        leermiddelenText.text = $"<link=\"{link}\">{vak}</link>";
    }
}
