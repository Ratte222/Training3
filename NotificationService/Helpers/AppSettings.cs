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

        public MongoDBSettings MongoDBSettings_PN { get; set; }
            = new MongoDBSettings();
        public TelegramBotSettings TelegramBotSettings { get; set; }
            = new TelegramBotSettings();
    }

    public class ConnectionStrings
    {
        public string MySQL { get; set; }
        public string PathToBinaryFile { get; set; }
        public string PathToJsonFile { get; set; }
        public string MySQL_PN { get; set; }
        public string PathToBinaryFile_PN { get; set; }
        public string PathToJsonFile_PN { get; set; }
    }

}
