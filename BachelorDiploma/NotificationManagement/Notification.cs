using Notifications.Wpf;
using System;

namespace BachelorDiploma.NotificationManagement
{
    public static class Notification
    {
        public static readonly NotificationManager notificationManager = new NotificationManager();

        public static void Show(string title, string message, NotificationType type, int hours, int minutes, int seconds)
        {
            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = type
            },
                expirationTime: new TimeSpan(hours, minutes, seconds));
        }
    }
}
