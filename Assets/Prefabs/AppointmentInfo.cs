using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AppointmentInfo : MonoBehaviour
{
    public void SetAppointmentInfo(string lokaal = null, string tijd = null, string docent = null, string vak = null, string lesUur = null)
    {
        if (!string.IsNullOrEmpty(lokaal))
        {
            this.lokaal.text = lokaal;
        }
        else
        {
            this.lokaal.text = "Geen lokaal";
        }
        
        if (!string.IsNullOrEmpty(tijd))
        {
            this.tijd.text = tijd;
        }
        else
        {
            this.tijd.text = "Geen tijd";
        }
        
        if (!string.IsNullOrEmpty(docent))
        {
            this.docent.text = docent;
        }
        else
        {
            this.docent.gameObject.SetActive(false);
        }
        
        if (!string.IsNullOrEmpty(vak))
        {
            this.vak.text = vak;
        }
        else
        {
            this.vak.gameObject.SetActive(false);
        }
        
        if (!string.IsNullOrEmpty(lesUur))
        {
            this.lesUur.text = lesUur;
        }
        else
        {
            this.lesUur.gameObject.SetActive(false);
        }
    }

    [SerializeField] TMP_Text lokaal;
    [SerializeField] TMP_Text tijd;
    [SerializeField] TMP_Text vak;
    [SerializeField] TMP_Text docent;
    [SerializeField] TMP_Text lesUur;
    public Schedule.Appointment _appointment;

    private void Start()
    {
        
    }
}
