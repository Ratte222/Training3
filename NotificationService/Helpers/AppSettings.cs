using NotificationService.Interfaces;
using NotificationService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Helpers
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
            = new ConnectionStrings();
        public IEmailConfiguration EmailConfiguration { get; set; }
            = new EmailConfiguration();
        public MongoDBSettings MongoDBSettings { get; set; }
            = new MongoDBSettings();
    }

    public class ConnectionStrings
    {
        public string MySQL { get; set; }
    }

}
