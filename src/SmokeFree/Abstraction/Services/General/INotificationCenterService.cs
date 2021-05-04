using System;

namespace SmokeFree.Abstraction.Services.General
{
    /// <summary>
    /// Cross Ploatform Notification Service Abstraction 
    /// </summary>
    public interface INotificationCenterService
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
        void ShowNewNotification(
            int notificationId,
            string title, 
            string message, 
            DateTime timeToShow,
            string returningData = "Dummy data",
            string icon = "icon");
    }
}
