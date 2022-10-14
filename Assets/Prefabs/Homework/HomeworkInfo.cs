using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeworkInfo : MonoBehaviour
{
    public void SetHomeworkInfo(string vak = null, string details = null, bool gemaakt = false, Homework.Item homeworkInfo = null)
    {
        if (vak?.Length > 40)
        {
            this.vak.text = vak.Substring(0, Math.Min(vak.Length, 37)) + "...";
        }
        else
        {
            this.vak.text = vak ?? "";
        }
        
        if (details?.Length > 68)
        {
            this.details.text = details.Substring(0, Math.Min(details.Length, 65)) + "...";
        }
        else
        {
            this.details.text = details ?? "";
        }
        
        this.gemaakt.isOn = gemaakt;
        
        if (homeworkInfo != null)
        {
            this.homeworkInfo = homeworkInfo;
        }
        
        if (homeworkInfo.studiewijzerItem.huiswerkType == "TOETS")
        {
            ToetsPill.SetActive(true); //
            GroteToetsPill.SetActive(false);
        }
        else if (homeworkInfo.studiewijzerItem.huiswerkType == "GROTE_TOETS")
        {
            ToetsPill.SetActive(false);
            GroteToetsPill.SetActive(true); //
        }
    }

    [SerializeField] TMP_Text vak;
    [SerializeField] TMP_Text details;
    [SerializeField] public Toggle gemaakt;
    
    [Space, SerializeField] private GameObject ToetsPill;
    [SerializeField] private GameObject GroteToetsPill;
    
    
    public Homework.Item homeworkInfo;
}
