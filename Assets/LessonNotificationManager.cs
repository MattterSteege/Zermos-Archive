using System;
using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;
using LocalNotification = EasyMobile.LocalNotification;

public class LessonNotificationManager : MonoBehaviour
{
    [SerializeField] Schedule schedule;
    List<Schedule.Appointment> _appointments;

    private void Start()
    {
#if !UNITY_EDITOR
        ViewManager.onInitializeComplete += ctx => onIntializeComplete();
#endif
    }

    void OnEnable()
    {
        ViewManager.onInitializeComplete += ctx => onIntializeComplete();
        Notifications.LocalNotificationOpened += OnLocalNotificationOpened;
    }
    
    void OnDisable()
    {
        ViewManager.onInitializeComplete -= ctx => onIntializeComplete();
        Notifications.LocalNotificationOpened -= OnLocalNotificationOpened;
    }
    
    private void onIntializeComplete()
    {
        _appointments = schedule.getScheduleOfDay(TimeManager.Instance.DateTime);

        // Cancels all pending local notifications.
        Notifications.CancelAllPendingLocalNotifications();
        
        if (Notifications.DataPrivacyConsent != ConsentStatus.Granted)
        {
            Notifications.GrantDataPrivacyConsent();
        }

        for (int i = 0; i < _appointments.Count; i++)
        {
            if (_appointments[i].appointmentType == "choice") continue;
            
            if (i == _appointments.Count - 1)
            {
                string title = $"Nog 5 minuten!";
                string body = $"Je laatste les is dan afgelopen!";
                
                ScheduleLocalNotification(title, body, UnixTimeStampToDateTime(_appointments[i].end).AddMinutes(-5));
            }
            else
            {
                string title = $"Nog 5 minuten!";
                string body = $"De volgende les is {_appointments[i].subjects?[0] ?? "error"} in {_appointments[i].locations?[0] ?? "error"}.";
                
                ScheduleLocalNotification(title, body, UnixTimeStampToDateTime(_appointments[i].end).AddMinutes(-5));
            }
        }
    }
    
    private void OnLocalNotificationOpened(LocalNotification delivered)
    {
        Notifications.ClearAllDeliveredNotifications();
    }
    
    void ScheduleLocalNotification(string title, string body, DateTime timeToSend)
    {
        NotificationContent content = PrepareNotificationContent(title, body);

        Notifications.ScheduleLocalNotification(timeToSend, content);
        
        NotificationContent PrepareNotificationContent(string title, string body)
        {
            NotificationContent content = new NotificationContent();
            
            content.title = title;
            content.body = body;

            return content;
        }
    }
    
    
    
    private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
}
