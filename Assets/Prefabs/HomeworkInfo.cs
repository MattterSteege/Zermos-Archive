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
        this.vak.text = vak ?? "";
        if (details.Length > 24)
        {
            this.details.text = details.Substring(0, Math.Min(details.Length, 21)) + "...";
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
