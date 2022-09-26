using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoosterItemView : View
{
    private Schedule.Appointment appointment;
    
    [SerializeField] TMP_Text lokaal;
    [SerializeField] TMP_Text tijd;
    [SerializeField] TMP_Text vak;
    [SerializeField] TMP_Text docent;
    
    private void Load()
    {
        appointment = (Schedule.Appointment)args;
        
        ViewManager.Instance.onLoad -= Load;
        
        Initialize();
    }
    
    public override void Initialize()
    {
        ViewManager.Instance.onLoad += Load;
        
        try
        {
            if (appointment.id != null)
            {
                lokaal.text = appointment.locations[0] ?? "Onbekend";
                tijd.text = DateTimeOffset.FromUnixTimeSeconds(appointment.start).AddHours(2).UtcDateTime.ToShortTimeString() + " - " +
                    DateTimeOffset.FromUnixTimeSeconds(appointment.end).AddHours(2).UtcDateTime.ToShortTimeString() ?? "Onbekend";
        
                vak.text = appointment.subjects[0] ?? "Onbekend";
                docent.text = appointment.teachers[0] ?? "Onbekend";
            }
            else if (appointment.actions[0].post != null)
            {
                
            }
            
        }
        catch(NullReferenceException){}
    }
}