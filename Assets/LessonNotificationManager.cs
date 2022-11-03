using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class LessonNotificationManager : MonoBehaviour
{
    [SerializeField] Schedule schedule;
    List<Schedule.Appointment> _appointments;

    private void Start()
    {
        if (PlayerPrefs.GetInt("notifssetup", 0) == 0)
        {
            var channel = new AndroidNotificationChannel()
            {
                Id = "lessons",
                Name = "lessons",
                Importance = Importance.Default,
                Description = "the default channel for sending lesson notifications"
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
            
            PlayerPrefs.SetInt("notifssetup", 1);
        }
    }

    private void OnApplicationQuit()
    {
        
        _appointments = schedule.getScheduleOfDay(TimeManager.Instance.DateTime);
        
        if (_appointments == null) return;

        // Cancels all pending local notifications.
        AndroidNotificationCenter.CancelAllNotifications();
        
        var firstlesson = _appointments.Find(x => x.appointmentType == "lesson" && x.status[0].code != 4007);
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        DateTime timeTillDeparture = dateTime.AddSeconds(firstlesson.start).ToLocalTime() - new TimeSpan(0, PlayerPrefs.GetInt("minutesbeforeclass", 1), 0);
        
        if (timeTillDeparture > DateTime.Now)
            ScheduleLocalNotification("Nog 5 minuten!", "Je moet bijna vertrekken.", timeTillDeparture.AddMinutes(-5));

        for (int i = 0; i < _appointments.Count; i++)
        {
            if (_appointments[i].appointmentType == "choice") continue;

            if (UnixTimeStampToDateTime(_appointments[i].start) < DateTime.Now) continue;

                if (i == _appointments.Count - 1)
            {
                string title = $"Nog 5 minuten!";
                string body = $"Je laatste les is dan afgelopen!";
                
                ScheduleLocalNotification(title, body, UnixTimeStampToDateTime(_appointments[i].end).AddMinutes(-5));
            }
            else
            {
                try
                {
                    string title = $"Nog 5 minuten!";
                    string body =
                        $"De volgende les is {_appointments[i + 1].subjects?[0] ?? "error"} in {_appointments[i + 1].locations?[0] ?? "error"}.";

                    ScheduleLocalNotification(title, body,
                        UnixTimeStampToDateTime(_appointments[i].end).AddMinutes(-5));
                }catch(Exception) { }
            }
        }
    }

    void ScheduleLocalNotification(string title, string body, DateTime timeToSend)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = body;
        notification.FireTime = timeToSend;

        AndroidNotificationCenter.SendNotification(notification, "lessons");
        
        Debug.Log($"Added notification for {timeToSend}");
    }
    
    [ContextMenu("Test")]
    public void SendTestNotification()
    {
        print("Test notif");
        
        var notification = new AndroidNotification();
        notification.Title = "Test";
        notification.Text = "Test";
        notification.FireTime = DateTime.Now.AddSeconds(5);

        AndroidNotificationCenter.SendNotification(notification, "lessons");
    }


    private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
}
