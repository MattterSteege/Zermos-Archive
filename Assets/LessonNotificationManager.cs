using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Notifications.Android;
using UnityEngine;

public class LessonNotificationManager : MonoBehaviour
{
    [SerializeField] Schedule schedule;
    List<Schedule.Appointment> _appointments;

    private void Start()
    {
        var channels = AndroidNotificationCenter.GetNotificationChannels();
        
        if (channels.All(x => x.Id != "lessons"))
        {
            var channel = new AndroidNotificationChannel
            {
                Id = "lessons",
                Name = "lessons",
                Importance = Importance.Default,
                Description = "the default channel for sending lesson notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
            
            Debug.Log("channel created");
        }
    }

    [ContextMenu("Test real")]
    public void OnApplicationQuit()
    {
        try
        {
            _appointments = schedule.getScheduleOfDay(DateTime.Today);
        }
        catch (Exception) { return; }

        if (_appointments == null) return;

        // Cancels all pending local notifications.
        AndroidNotificationCenter.CancelAllNotifications();
        
        var firstlesson = _appointments.Find(x => x.appointmentType == "lesson" && x.status[0].code != 4007);
        if (firstlesson == null) return;
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        DateTime timeTillDeparture = dateTime.AddSeconds(firstlesson.start).ToLocalTime() - new TimeSpan(0, PlayerPrefs.GetInt("minutesbeforeclass", 1), 0);
        
        if (timeTillDeparture > DateTime.Now)
            ScheduleLocalNotification("Nog 5 minuten!", "Je moet bijna vertrekken.", timeTillDeparture.AddMinutes(-5));

        for (int i = 0; i < _appointments.Count; i++)
        {
            string title = "";
            string body = "";
            
            if (_appointments[i].appointmentType == "choice") continue;

            if (_appointments[i].start.ToDateTime() < DateTime.Now) continue;

            if (i == _appointments.Count - 1)
            {
                title = $"Nog 5 minuten!";
                body = $"De laatste les is {_appointments[i].subjects?[0] ?? "error"} in {_appointments[i].locations?[0] ?? "error"}.";

                ScheduleLocalNotification(title, body, _appointments[i].start.ToDateTime().AddMinutes(-5));
                
                title = $"Nog 5 minuten!";
                body = $"Je laatste les is dan afgelopen!";
                
                ScheduleLocalNotification(title, body, _appointments[i].end.ToDateTime().AddMinutes(-5));
                
                break;
            }
            
            title = $"Nog 5 minuten!";
            body = $"De volgende les is {_appointments[i].subjects?[0] ?? "error"} in {_appointments[i].locations?[0] ?? "error"}.";

            ScheduleLocalNotification(title, body, _appointments[i].start.ToDateTime().AddMinutes(-5));
        }
    }

    void ScheduleLocalNotification(string title, string body, DateTime timeToSend)
    {
        DateTime now = DateTime.Now;
        AndroidNotification notification = new AndroidNotification
        {
            Title = title,
            Text = body,
            FireTime = timeToSend,
            IntentData = "{\"title\": \"Notification 1\", \"data\": \"200\"}",
            ShouldAutoCancel = true,
            ShowTimestamp = true,
        };

        AndroidNotificationCenter.SendNotification(notification, "lessons");
        
        Debug.Log($"Added notification for {timeToSend},\n{title}, {body}");
    }
    
    [ContextMenu("Test")]
    public void SendTestNotification()
    {
        print("Test notif");
        
        ScheduleLocalNotification("Test", "Test", DateTime.Now.AddSeconds(5));
    }
}
