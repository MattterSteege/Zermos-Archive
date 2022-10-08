using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeworkInfo : MonoBehaviour
{
    public void SetHomeworkInfo(string vak = null, string details = null, bool gemaakt = false)
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
    }

    [SerializeField] TMP_Text vak;
    [SerializeField] TMP_Text details;
    [SerializeField] Toggle gemaakt;
}
