using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    public interface INotificationSenderSettings
    {

        /// <summary>
        /// time between processing of the notification queue in second
        /// </summary>
        int SleepTimeNotification { get; set; }
        /// <summary>
        /// the nomber of unsuccessfull attempt to send a notification
        /// before writing a notification to a file 
        /// </summary>
        int NumberOfAttemptToSentForPushToFile {get;set;}
        /// <summary>
        /// difference between the time of the last attempt to send notification and
        /// the current time in minutes before write a notification to a file
        /// </summary>
        int TimeOfTheLastAttemptToSendForPushToFile {get;set;}
        /// <summary>
        /// difference between create time notification and the current time 
        /// in minutes before a writing notification to a file  
        /// </summary>
        int TimeAfterCreateForPushToFile{get;set;}
    }
}
