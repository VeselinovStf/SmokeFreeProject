using System;

namespace SmokeFree.Abstraction.Managers
{
    /// <summary>
    ///  Cross-platform API that the application can use to interact with local notifications
    /// </summary>
    public interface INotificationManager
    {
        /// <summary>
        ///  Handle incoming notifications
        /// </summary>
        event EventHandler NotificationReceived;

        /// <summary>
        ///  Perform any native platform logic needed to prepare the notification system
        /// </summary>
        void Initialize();

        /// <summary>
        /// Send a notification
        /// </summary>
        /// <param name="title">Notification Title</param>
        /// <param name="message">Notification Message</param>
        /// <param name="notifyTime">At an optional DateTime</param>
        void SendNotification(string title, string message, DateTime? notifyTime = null);

        /// <summary>
        /// Called by the underlying platform when a message is received.
        /// </summary>
        /// <param name="title">Notification Title</param>
        /// <param name="message">Notification Message</param>
        void ReceiveNotification(string title, string message);
    }
}
