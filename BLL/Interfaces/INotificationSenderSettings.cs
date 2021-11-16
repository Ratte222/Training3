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
    }
}
