using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Training3.NotificationServiceConfiguration
{
    public class NotificationSenderSettings 
    {
        public int SleepTimeNotification { get; set; }
        public int NumberOfAttemptToSentForPushToFile { get; set; }
        public int TimeOfTheLastAttemptToSendForPushToFile { get; set; }
        public int TimeAfterCreateForPushToFile { get; set; }
        public bool StopProblemNotificationServiceAfterException { get; set; }
        public bool SendMailIfProblemNotificationServiceHasException { get; set; }
        public bool StopNotificationServiceSenderAfterException { get; set; }
        public bool SendMailIfNotificationServiceSenderHasException { get; set; }
        public string DeveloperEmail { get; set; }
        public QueueDatabaseType QueueDatabaseType { get; set; }
    }
    public enum QueueDatabaseType
    {
        MySQL = 0,
        InMemory
    }
}
