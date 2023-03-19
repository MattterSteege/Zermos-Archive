using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeworkInfo : MonoBehaviour
{
    [SerializeField] TMP_Text vak;
    //[SerializeField] TMP_Text tijd;
    [SerializeField] TMP_Text details;
    [SerializeField] TMP_Text Datum;
    [SerializeField] public Toggle gemaakt;
    
    [Space, SerializeField] private GameObject notificationPill;

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

        //this.tijd.text = TimeManager.Instance.DateTime.GetDateDifference(date).GetTimeDifferenceString();

        this.gemaakt.isOn = gemaakt;
        this.gemaakt.GetComponentInChildren<TMP_Text>().text = gemaakt ? "Voltooid" : "Onvoltooid";
        
        if (homeworkInfo != null)
        {
            this.homeworkInfo = homeworkInfo;
        }
        
        //true when: test, big test, or 1 or more attachments
        notificationPill.SetActive(homeworkInfo.studiewijzerItem.huiswerkType == "TOETS" || homeworkInfo.studiewijzerItem.huiswerkType == "GROTE_TOETS" || homeworkInfo.studiewijzerItem.bijlagen?.Count > 0);
    }
}
