﻿using System;

namespace SmokeFree.Models.Managers.NotificationManager
{
    public class NotificationEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
