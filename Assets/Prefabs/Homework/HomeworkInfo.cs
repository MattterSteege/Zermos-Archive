using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeworkInfo : MonoBehaviour
{
    [SerializeField] TMP_Text vak;
    [SerializeField] TMP_Text details;
    [SerializeField] TMP_Text Datum;
    [SerializeField] public Toggle gemaakt;
    
    [Space, SerializeField] private GameObject toetsPill;
    [SerializeField] private GameObject groteToetsPill;
    [SerializeField] private GameObject bijlagePill;

    public Homework.Item homeworkInfo;
    
    public void SetHomeworkInfo(string vak = null, string details = null, DateTime date = default, bool gemaakt = false, Homework.Item homeworkInfo = null)
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
        
        if (date.Date == TimeManager.Instance.DateTime.Date)
        {
            this.Datum.text = "Vandaag";
        }
        else if (date.Date == TimeManager.Instance.DateTime.Date.AddDays(1))
        {
            this.Datum.text = "Morgen";
        }
        else if (date.Date == TimeManager.Instance.DateTime.Date.AddDays(2))
        {
            this.Datum.text = "Overmorgen";
        }
        else if (date.Date == TimeManager.Instance.DateTime.Date.AddDays(-1))
        {
            this.Datum.text = "Gisteren";
        }
        else if (date.Date == TimeManager.Instance.DateTime.Date.AddDays(-2))
        {
            this.Datum.text = "Eergisteren";
        }
        else
        {
            this.Datum.text = date.ToString("dd-MM-yyyy");
        }

        this.gemaakt.isOn = gemaakt;
        
        if (homeworkInfo != null)
        {
            this.homeworkInfo = homeworkInfo;
        }
        
        if (homeworkInfo.studiewijzerItem.huiswerkType == "TOETS")
        {
            toetsPill.SetActive(true);
            groteToetsPill.SetActive(false);
        }
        else if (homeworkInfo.studiewijzerItem.huiswerkType == "GROTE_TOETS")
        {
            toetsPill.SetActive(false);
            groteToetsPill.SetActive(true);
        }

        bijlagePill.SetActive(homeworkInfo.studiewijzerItem.bijlagen?.Count > 0);
    }
}
