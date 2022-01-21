using CommonForCryptPasswordLibrary.WorkWithJson;
using Newtonsoft.Json;
using NotificationService.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Training3.NotificationServiceConfiguration
{
    public class NotificationServiceBuilder
    {
        private string PathToNotificationService { get; set; }
        public NotificationSenderSettings NotificationSenderSettings { get; set; }
            = new NotificationSenderSettings();
        public AppSettings AppSettings { get; set; }
            = new AppSettings();
        
        public NotificationServiceBuilder(string pathToNotificationService)
        {
            _ = pathToNotificationService ?? throw new ArgumentNullException($"{nameof(pathToNotificationService)} is null or empty");
            PathToNotificationService = pathToNotificationService;
            SetDefaultSettings();
        }

        

        public NotificationServiceProcess Build_UpdateConfiguration()
        {
            string pathToConfigs = Path.GetDirectoryName(PathToNotificationService);
            SerializeDeserializeJson<NotificationService.Helpers.AppSettings> serializeAppSettings =
                new SerializeDeserializeJson<AppSettings>();
            serializeAppSettings.SerializeToFile(AppSettings, Path.Combine(
                pathToConfigs, NotificationService.Startup.AppSettingsFileName));
            SerializeDeserializeJson<NotificationSenderSettings> serializeNotificationSenderService =
                new SerializeDeserializeJson<NotificationSenderSettings>();
            string jsonConfig = $"{{ \"{nameof(NotificationSenderSettings)}\" : " +
                $"{serializeNotificationSenderService.Serialize(NotificationSenderSettings)} }}";
            File.WriteAllText(Path.Combine(
                pathToConfigs, NotificationService.Startup.ConfigureNotificationServiceFileName), jsonConfig);
            var process = new NotificationServiceProcess(PathToNotificationService);
            return process;
        }

        private void SetDefaultSettings()
        {
            NotificationSenderSettings.SleepTimeNotification = 10;
            NotificationSenderSettings.NumberOfAttemptToSentForPushToFile = 5;
            NotificationSenderSettings.TimeOfTheLastAttemptToSendForPushToFile = 5;
            NotificationSenderSettings.TimeAfterCreateForPushToFile = 1;
            NotificationSenderSettings.StopNotificationServiceSenderAfterException = true;
            NotificationSenderSettings.StopProblemNotificationServiceAfterException = true;
            NotificationSenderSettings.SendMailIfNotificationServiceSenderHasException = false;
            NotificationSenderSettings.SendMailIfProblemNotificationServiceHasException = false;
            NotificationSenderSettings.DeveloperEmail = null;
            NotificationSenderSettings.QueueDatabaseType = NotificationService.Constants.QueueDatabaseType.InMemory;
            //TODO: delete this settings
            AppSettings.MongoDBSettings.ConnectionString = "mongodb://localhost:27017";
            AppSettings.MongoDBSettings.DatabaseName = "Training3DB";
            AppSettings.MongoDBSettings.NotificationDatabaseName = "Notifications";
            AppSettings.MongoDBSettings_PN.ConnectionString = "mongodb://localhost:27017";
            AppSettings.MongoDBSettings_PN.DatabaseName = "Training3DB_PN";
            AppSettings.MongoDBSettings_PN.NotificationDatabaseName = "Notifications";
        }
    }
}
