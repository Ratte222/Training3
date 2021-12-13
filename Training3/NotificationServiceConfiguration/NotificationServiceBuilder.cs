using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Training3.NotificationServiceConfiguration
{
    public class NotificationServiceBuilder
    {
        public NotificationSenderSettings NotificationSenderSettings { get; set; }
            = new NotificationSenderSettings();
        public AppSettings AppSettings { get; set; }
            = new AppSettings();
    }
}
