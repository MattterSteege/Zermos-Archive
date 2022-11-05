using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoosterItemView : View
{
    [SerializeField] private Schedule.Appointment appointment;
    
    [SerializeField] TMP_Text lokaal;
    [SerializeField] TMP_Text tijd;
    [SerializeField] TMP_Text vak;
    [SerializeField] TMP_Text docent;
    [SerializeField] Image background;
    [SerializeField] private GameObject inplanLesPrefab;
    [SerializeField] private GameObject inplanLesContainer;
    [SerializeField] private TMP_Text Title;

    public override void Show(object args = null)
    {
        this.appointment = (Schedule.Appointment)args;
        
        Initialize();
        
        base.Show();
    }
    
    public override void Initialize()
    {
        openNavigationButton.onClick.RemoveAllListeners();
        openNavigationButton.onClick.AddListener(() =>
        {
            ViewManager.Instance.ShowNewView<DagRoosterView>();
        });
        
        foreach (Transform child in inplanLesContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
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
                Title.text = subjects[0];
            }
            
            if (appointment.id != null)
            {
                lokaal.text = String.Join(", ", locations);
                tijd.text = DateTimeOffset.FromUnixTimeSeconds(appointment.start).AddHours(2).UtcDateTime.ToShortTimeString() + " - " +
                    DateTimeOffset.FromUnixTimeSeconds(appointment.end).AddHours(2).UtcDateTime.ToShortTimeString() ?? "Onbekend";
        
                vak.text = String.Join(", ", subjects);;
                docent.text = String.Join(", ", teachers);;
            }
            
            if (appointment.actions.Count > 0)
            {
                for (int i = 0; i < appointment.actions.Count; i++)
                {
                    if (appointment.actions[i].allowed == false) continue;
                    
                    GameObject inplanLes = Instantiate(inplanLesPrefab, inplanLesContainer.transform);
                    inplanLes.GetComponent<InPlanLes>().post = appointment.actions[i].post;
                    
                    inplanLes.GetComponent<AppointmentInfo>().SetAppointmentInfo(String.Join(", ", appointment.actions[i].appointment.locations),
                        DateTimeOffset.FromUnixTimeSeconds(appointment.actions[i].appointment.start).AddHours(2).UtcDateTime
                            .ToShortTimeString() + " - " + DateTimeOffset.FromUnixTimeSeconds(appointment.actions[i].appointment.end)
                            .AddHours(2).UtcDateTime
                            .ToShortTimeString(), String.Join(", ", appointment.actions[i].appointment.teachers), String.Join(", ", appointment.actions[i].appointment.subjects), appointment.actions[i].appointment.startTimeSlotName, appointment.actions[i].appointment);
                    
                    inplanLes.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        inplanLes.GetComponent<InPlanLes>().enrollIntoLesson();
                    });
                }
            }

            if (appointment.status.Count > 0 && appointment.status[0].code == 4007)
            {
                background.color = new Color(1f, 0f, 0f, 0.5f);
            }
        }
        catch(NullReferenceException){}
    }
}