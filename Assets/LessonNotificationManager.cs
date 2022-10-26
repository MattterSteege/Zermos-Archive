using System;
using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;

public class LessonNotificationManager : MonoBehaviour
{
    public void Start()
    {
        //ViewManager.onInitializeComplete += onIntializeComplete;
    }

    private void onIntializeComplete()
    {
        if (Notifications.DataPrivacyConsent != ConsentStatus.Granted)
        {
            Notifications.GrantDataPrivacyConsent();
        }

        foreach (Schedule.Appointment appointment in Schedule.ScheduledAppointments.response.data[0].appointments)
        {
            string title = $"Nog 5 minuten!";
            string body = $"De volgende les is {appointment.subjects?[0] ?? "error"} in {appointment.locations?[0] ?? "error"}.";
            
            ScheduleLocalNotification(title, body, UnixTimeStampToDateTime(appointment.start).AddMinutes(-5));
        }
        
    }
    
    void ScheduleLocalNotification(string title, string body, DateTime timeToSend)
    {
        NotificationContent content = PrepareNotificationContent(title, body);

        Notifications.ScheduleLocalNotification(timeToSend, content);
    }
    
    
    // Construct the content of a new notification for scheduling.
    NotificationContent PrepareNotificationContent(string title, string body)
    {
        NotificationContent content = new NotificationContent();
        
        content.title = title;
        content.body = body;

        //content.smallIcon = "YOUR_CUSTOM_SMALL_ICON";
        
        return content;
    }
    
    private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
}
