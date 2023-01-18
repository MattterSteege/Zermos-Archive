using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class InPlanLesView : View
    {
        [SerializeField] private Schedule.Appointment appointment;
        
        [SerializeField] private GameObject inplanLesPrefab;
        [SerializeField] private GameObject inplanLesContainer;
        [SerializeField] private TMP_Text Title;

        public override void Show(object args = null)
        {
            this.appointment = (Schedule.Appointment)args;
            
            foreach (Transform child in inplanLesContainer.transform)
            {
                Destroy(child.gameObject);
            }
        
            try
            {
                if (appointment != null && appointment.actions.Count > 0)
                {
                    foreach (var appointment in appointment.actions)
                    {
                        if (appointment.allowed == false) continue;
                    
                        GameObject inplanLes = Instantiate(inplanLesPrefab, inplanLesContainer.transform);
                        inplanLes.GetComponent<InPlanLes>().post = appointment.post;
                    
                        inplanLes.GetComponent<AppointmentInfo>().SetAppointmentInfo(String.Join(", ", appointment.appointment.locations),
                            DateTimeOffset.FromUnixTimeSeconds(appointment.appointment.start).AddHours(2).UtcDateTime
                                .ToShortTimeString() + " - " + DateTimeOffset.FromUnixTimeSeconds(appointment.appointment.end)
                                .AddHours(2).UtcDateTime
                                .ToShortTimeString(), String.Join(", ", appointment.appointment.teachers), String.Join(", ", appointment.appointment.subjects), appointment.appointment.startTimeSlotName, appointment.appointment);
                    
                        inplanLes.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            inplanLes.GetComponent<InPlanLes>().enrollIntoLesson();
                        });
                    }
                }
            }
            catch(NullReferenceException){}
            
            base.Show();
        }
        
    
        public override void Initialize()
        {
            openNavigationButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.ShowNewView<DagRoosterView>();
            });
        }
        
        public override void Refresh(object args)
        {
            openNavigationButton.onClick.RemoveAllListeners();
            closeButtonWholePage.onClick.RemoveAllListeners();
            base.Refresh(args);
        }
    }
}