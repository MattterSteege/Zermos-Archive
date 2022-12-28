using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;
using UnityEngine.Android;

public class LessonNotificationManager : MonoBehaviour
{
#if UNITY_ANDROID

    [SerializeField] Schedule schedule;
    List<Schedule.Appointment> _appointments;
    
    const string CHANNEL_ID = "lessons";

    private void Start()
    {
        ViewManager.onInitializeComplete += ctx => ScheduleNotifications();
    }

    [ContextMenu("Test real")]
    public void ScheduleNotifications()
    {
        Debug.Log("Adding notifications");
        
#region Permissions & Channel
        var channels = AndroidNotificationCenter.GetNotificationChannels();

        if (channels.All(x => x.Id != CHANNEL_ID))
        {
            var channel = new AndroidNotificationChannel()
            {
                Id = CHANNEL_ID,
                Name = "Lessons",
                Importance = Importance.High,
                Description = "Notifications for lessons"
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
            Debug.Log("channel created");
        }
#endregion

        try
        {
            _appointments = schedule.GetScheduleOfDay(TimeManager.Instance.DateTime);
        }
        catch (Exception)
        {
            Debug.Log("Error fetching schedule for notifs");
        }

        if (_appointments == null) return;

        // Cancels all pending local notifications.
        AndroidNotificationCenter.CancelAllNotifications();
        
        var firstlesson = _appointments.Find(x => x.appointmentType == "lesson" && x.status[0].code != 4007);
        if (firstlesson == null) return;
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        DateTime timeTillDeparture = dateTime.AddSeconds(firstlesson.start).ToLocalTime() - new TimeSpan(0, LocalPrefs.GetInt("minutes_before_class", 1), 0);
        
        if (timeTillDeparture > TimeManager.Instance.CurrentDateTime)
            ScheduleLocalNotification("Nog 5 minuten!", "Je moet bijna vertrekken.", timeTillDeparture.AddMinutes(-5));

        for (int i = 0; i < _appointments.Count; i++)
        {
            string title = "";
            string body = "";
            
            
            if (_appointments[i].appointmentType == "choice") continue;

            if (_appointments[i].start <= TimeManager.Instance.CurrentDateTime.ToUnixTime()) continue;

            if (i == _appointments.Count - 1)
            {
                title = $"Nog 5 minuten!";
                body = $"De laatste les is {_appointments[i].subjects?[0] ?? "error"} in {_appointments[i].locations?[0] ?? "error"}.";

                ScheduleLocalNotification(title, body, TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(_appointments[i].start), TimeZoneInfo.Local).DateTime.AddMinutes(-5));
                
                title = $"Nog 5 minuten!";
                body = $"Je laatste les is dan afgelopen!";
                
                ScheduleLocalNotification(title, body, TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(_appointments[i].end), TimeZoneInfo.Local).DateTime.AddMinutes(-5));
                
                break;
            }
            
            title = $"Nog 5 minuten!";
            body = $"De volgende les is {_appointments[i].subjects?[0] ?? "error"} in {_appointments[i].locations?[0] ?? "error"}.";
            
            ScheduleLocalNotification(title, body, TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(_appointments[i].start), TimeZoneInfo.Local).DateTime.AddMinutes(-5));
        }
    }

    void ScheduleLocalNotification(string title, string body, DateTime timeToSend)
    {
        AndroidNotification notification = new AndroidNotification
        {
            Title = title,
            Text = body,
            ShouldAutoCancel = true,
            FireTime = timeToSend,
            ShowTimestamp = true,
        };

        AndroidNotificationCenter.SendNotification(notification, "lessons");
        
        Debug.Log($"Added notification for {timeToSend:HH:mm:ss}, {title}, {body}");
    }
    
    [ContextMenu("Test")]
    public void SendTestNotification()
    {
        print("Test notif");
        
        ScheduleLocalNotification("Test", "Test", TimeManager.Instance.CurrentDateTime.AddSeconds(5));
    }
#endif
}
