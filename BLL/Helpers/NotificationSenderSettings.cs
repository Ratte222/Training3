using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Helpers
{
    public class NotificationSenderSettings: INotificationSenderSettings
    {
        public int SleepTimeNotification { get; set; }
        public int NumberOfAttemptToSentForPushToFile {get;set;}
        public int TimeOfTheLastAttemptToSendForPushToFile {get;set;}
        public int TimeAfterCreateForPushToFile {get;set;}
    }
}
