using Plugin.LocalNotification;
using SmokeFree.Abstraction.Services.General;
using System;

namespace SmokeFree.Services.General
{
    /// <summary>
    /// Cross Ploatform Notification Service 
    /// </summary>
    public class NotificationCenterService : INotificationCenterService
    {
        /// <summary>
        /// Show Notification
        /// </summary>
        /// <param name="notificationId">Unique Notification Id</param>
        /// <param name="title">Title of message</param>
        /// <param name="message">Message Content</param>
        /// <param name="timeToShow">Time to show</param>
        /// <param name="returningData">Data to return</param>
        /// <param name="icon">Notification icon</param>
        public void ShowNewNotification(int notificationId, string title, string message, DateTime timeToShow, string returningData = "Dummy data", string icon = "icon")
        {
            var testTimerNotification = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = title,
                Description = message,
                ReturningData = returningData, // Returning data when tapped on notification.
                NotifyTime = timeToShow,
                Android = new AndroidOptions()
                {
                    IconName = icon
                } // Used for Scheduling local notification, if not specified notification will show immediately.
            };

            NotificationCenter.Current.Show(testTimerNotification);
        }
    }
}
