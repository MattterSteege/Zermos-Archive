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
            //initialize list<string> named appointments with one value in it
            List<string> locations = new List<string> {"No Location(s)"};
            List<string> teachers = new List<string> {"No Teacher(s)"};
            List<string> subjects = new List<string> {"No Subject(s)"};

            if (appointment.locations.Count > 0)
            {
                locations = appointment.locations;
            }
                    
            if (appointment.teachers.Count > 0)
            {
                teachers = appointment.teachers;
            }
                    
            if (appointment.subjects.Count > 0)
            {
                subjects = appointment.subjects;
            }
            
            if (appointment.id != null)
            {
                lokaal.text = String.Join(", ", locations);
                tijd.text = DateTimeOffset.FromUnixTimeSeconds(appointment.start).AddHours(2).UtcDateTime.ToShortTimeString() + " - " +
                    DateTimeOffset.FromUnixTimeSeconds(appointment.end).AddHours(2).UtcDateTime.ToShortTimeString() ?? "Onbekend";
        
                vak.text = String.Join(", ", subjects);;
                docent.text = String.Join(", ", teachers);;
            }
            else if (appointment.actions[0].post != null)
            {
                
            }
            
        }
        catch(NullReferenceException){}
    }
}