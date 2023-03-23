using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AntoniusNieuwsPaneel : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;
    
    public void SetTitle(string title)
    {
        this.title.text = title;
    }
    
    public void SetDescription(string description)
    {
        this.description.text = description;
    }
    
    public void SetTitleAndDescription(string title, string description)
    {
        SetTitle(title);
        SetDescription(description);
    }
}
