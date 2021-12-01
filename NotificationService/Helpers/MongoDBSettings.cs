using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Helpers
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string NotificationDatabaseName { get; set; }
    }
}
