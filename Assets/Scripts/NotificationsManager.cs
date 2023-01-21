using System;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{
    public void ScheduleLocalNotification(string title, string body, DateTime timeToSend)
    {
        TimeSpan timeSpan = timeToSend - TimeManager.Instance.CurrentDateTime;
        
        var notificationParams = new NotificationParams
        {
            Id = UnityEngine.Random.Range(0, int.MaxValue),
            Delay = timeSpan,
            Title = title,
            Message = body,
            Sound = false,
            Vibrate = true,
            Light = false,
            SmallIcon = NotificationIcon.Event,
            LargeIcon = ""
        };
        
        NotificationManager.SendCustom(notificationParams);
    }

    public void CancelAll()
    {
        NotificationManager.CancelAll();
    }
}