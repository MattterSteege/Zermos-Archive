using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Zermos_Web.Utilities
{
    public static class NotificationCenter
    {
        public enum NotificationType
        {
            INFO,
            WARNING,
            ERROR
        }

        public static void AddNotification(this HttpContext context, string title, string body, NotificationType type)
        {
            var notification = new Notification
            {
                Title = title,
                Body = body,
                Type = type.ToString().ToLower()
            };

            List<Notification> notifications = context.GetNotifications();
            notifications.Add(notification);

            context.Items["Notifications"] = notifications;
        }

        public static List<Notification> GetNotifications(this HttpContext context)
        {
            if (context != null)
            {
                if (context.Items["Notifications"] is List<Notification> notifications)
                    return notifications;

                return new List<Notification>();
            }
            
            return new List<Notification>();
        }
    }

    public class Notification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
    }


}