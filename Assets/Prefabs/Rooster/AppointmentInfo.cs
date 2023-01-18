using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AppointmentInfo : MonoBehaviour
{
    public void SetAppointmentInfo(string lokaal = "", string tijd = null, string docent = null, string vak = null, string lesUur = null, Schedule.Appointment appointment = null)
    {
        this.lokaal.text = !string.IsNullOrEmpty(lokaal) ? lokaal : "Geen lokaal";
        this.tijd.text = tijd;

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
        
        if (kwtUurAvailable != null)
            kwtUurAvailable.SetActive(appointment?.actions?.Count > 0);
        
        if (appointment != null)
        {
            this._appointment = appointment;
        }
    }

    [SerializeField] TMP_Text lokaal;
    [SerializeField] TMP_Text tijd;
    [SerializeField] TMP_Text vak;
    [SerializeField] TMP_Text docent;
    [SerializeField] TMP_Text lesUur;
    [SerializeField] GameObject kwtUurAvailable;
    public Schedule.Appointment _appointment;
    
    public void hide()
    {
        this.gameObject.SetActive(false);
    }
}
