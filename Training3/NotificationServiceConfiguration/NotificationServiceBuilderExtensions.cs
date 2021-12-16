using NotificationService.Constants;
using NotificationService.Helpers;
using NotificationService.Interfaces;
using System;


namespace Training3.NotificationServiceConfiguration
{
    public static class NotificationServiceBuilderExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationServiceBuilder"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static NotificationServiceBuilder UseMySQL(this NotificationServiceBuilder notificationServiceBuilder, 
            string connectionString)
        {
            _ = notificationServiceBuilder ?? throw new NullReferenceException(
                $"{nameof(notificationServiceBuilder)} is null");
            _ = connectionString ?? throw new NullReferenceException(
                $"{nameof(connectionString)} is null");
            notificationServiceBuilder.NotificationSenderSettings.QueueDatabaseType 
                = QueueDatabaseType.MySQL;
            notificationServiceBuilder.AppSettings.ConnectionStrings.MySQL = connectionString;
            return notificationServiceBuilder;
        }

        public static NotificationServiceBuilder UseDbInMemory(this NotificationServiceBuilder notificationServiceBuilder)
        {
            _ = notificationServiceBuilder ?? throw new NullReferenceException(
                $"{nameof(notificationServiceBuilder)} is null");
            notificationServiceBuilder.NotificationSenderSettings.QueueDatabaseType
                = QueueDatabaseType.InMemory;
            return notificationServiceBuilder;
        }

        /// <exception cref="NullReferenceException"></exception>
        public static NotificationServiceBuilder UseBinaryFile(this NotificationServiceBuilder notificationServiceBuilder,
            string pathToJsonFile)
        {
            _ = notificationServiceBuilder ?? throw new NullReferenceException(
                $"{nameof(notificationServiceBuilder)} is null");
            _ = pathToJsonFile ?? throw new NullReferenceException(
                $"{nameof(pathToJsonFile)} is null");
            notificationServiceBuilder.NotificationSenderSettings.QueueDatabaseType
                = QueueDatabaseType.InBinaryFile;
            notificationServiceBuilder.AppSettings.ConnectionStrings.PathToBinaryFile = pathToJsonFile;
            return notificationServiceBuilder;
        }

        /// <exception cref="NullReferenceException"></exception>
        public static NotificationServiceBuilder UseJsonFile(this NotificationServiceBuilder notificationServiceBuilder,
            string pathToJsonFile)
        {
            _ = notificationServiceBuilder ?? throw new NullReferenceException(
                $"{nameof(notificationServiceBuilder)} is null");
            _ = pathToJsonFile ?? throw new NullReferenceException(
                $"{nameof(pathToJsonFile)} is null");
            notificationServiceBuilder.NotificationSenderSettings.QueueDatabaseType
                = QueueDatabaseType.InJsonFile;
            notificationServiceBuilder.AppSettings.ConnectionStrings.PathToJsonFile = pathToJsonFile;
            return notificationServiceBuilder;
        }

        /// <summary>
        /// will stop the problemNotificationService if an error occurs in it
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static NotificationServiceBuilder StopProblemNotificationServiceAfterException(
            this NotificationServiceBuilder builder, bool parametr = false)
        {
            _ = builder ?? throw new NullReferenceException(
                $"{nameof(builder)} is null");
            builder.NotificationSenderSettings.StopProblemNotificationServiceAfterException
                = parametr;
            return builder;
        }

        /// <summary>
        /// will stop the NotificationService if an error occurs in it
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static NotificationServiceBuilder StopNotificationServiceAfterException(
            this NotificationServiceBuilder builder, bool parametr = false)
        {
            _ = builder ?? throw new NullReferenceException(
                $"{nameof(builder)} is null");
            builder.NotificationSenderSettings.StopNotificationServiceSenderAfterException
                = parametr;
            return builder;
        }

        public static NotificationServiceBuilder SendMailIfProblemNotificationServiceHasException(
            this NotificationServiceBuilder builder, string sendToEMail = null,
            IEmailConfiguration emailConfiguration = null)
        {
            _ = builder ?? throw new NullReferenceException(
                $"{nameof(builder)} is null");
            builder.NotificationSenderSettings.SendMailIfProblemNotificationServiceHasException
                = true;
            SetCredenials(builder, sendToEMail, emailConfiguration);
            return builder;
        }

        public static NotificationServiceBuilder SendMailIfNotificationServiceSenderHasException(
            this NotificationServiceBuilder builder, string sendToEMail = null,
            IEmailConfiguration emailConfiguration = null)
        {
            _ = builder ?? throw new NullReferenceException(
                $"{nameof(builder)} is null");
            builder.NotificationSenderSettings.SendMailIfNotificationServiceSenderHasException
                = true;
            SetCredenials(builder, sendToEMail, emailConfiguration);
            return builder;
        }

        private static void SetCredenials(NotificationServiceBuilder builder, string sendToEMail,
            IEmailConfiguration emailConfiguration)
        {
            if (emailConfiguration != null && !String.IsNullOrEmpty(sendToEMail))
            {
                builder.NotificationSenderSettings.DeveloperEmail = sendToEMail;
                builder.AppSettings.EmailConfiguration = emailConfiguration;
            }
        }


        /// <summary>
        /// difference between create time notification and the current time 
        /// in minutes before a writing notification to a file  
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="timeAfterCreateForPushToFile">in minute</param>
        /// <returns></returns>
        public static NotificationServiceBuilder SetTimeAfterCreateForPushToFile(this NotificationServiceBuilder builder,
            int timeAfterCreateForPushToFile)
        {
            _ = builder ?? throw new NullReferenceException(
                $"{nameof(builder)} is null");
            if (timeAfterCreateForPushToFile < 3) { timeAfterCreateForPushToFile = 3; }
            builder.NotificationSenderSettings.TimeAfterCreateForPushToFile = timeAfterCreateForPushToFile;
            return builder;
        }

        /// <summary>
        /// time between processing of the notification queue in second
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sleepTime">in minute</param>
        /// <returns></returns>
        public static NotificationServiceBuilder SetSleepTimeNotification(this NotificationServiceBuilder builder,
            int sleepTime)
        {
            _ = builder ?? throw new NullReferenceException(
                $"{nameof(builder)} is null");
            if (sleepTime < 3) { sleepTime = 3; }
            builder.NotificationSenderSettings.TimeAfterCreateForPushToFile = sleepTime;
            return builder;
        }

        /// <summary>
        /// the nomber of unsuccessfull attempt to send a notification
        /// before writing a notification to a file 
        /// </summary>
        /// <param name="builder"></param>        
        /// <returns></returns>
        public static NotificationServiceBuilder SetNumberOfAttemptToSentForPushToFile(this NotificationServiceBuilder builder,
            int numberOfAttempt)
        {
            _ = builder ?? throw new NullReferenceException(
                $"{nameof(builder)} is null");
            if (numberOfAttempt < 0) { numberOfAttempt = 0; }
            builder.NotificationSenderSettings.TimeAfterCreateForPushToFile = numberOfAttempt;
            return builder;
        }

        /// <summary>
        /// difference between the time of the last attempt to send notification and
        /// the current time in minutes before write a notification to a file
        /// </summary>
        /// <param name="time">in minute</param>        
        /// <returns></returns>
        public static NotificationServiceBuilder SetTimeOfTheLastAttemptToSendForPushToFile(this NotificationServiceBuilder builder,
            int time)
        {
            _ = builder ?? throw new NullReferenceException(
                $"{nameof(builder)} is null");
            if (time < 0) { time = 0; }
            builder.NotificationSenderSettings.TimeAfterCreateForPushToFile = time;
            return builder;
        }

        /// <summary>
        /// set up database for failed notifications 
        /// </summary>
        /// <returns></returns>
        public static NotificationServiceBuilder SetMongoSettings(this NotificationServiceBuilder builder,
            MongoDBSettings mongoDBSettings)
        {
            _ = builder ?? throw new NullReferenceException(
                $"{nameof(builder)} is null");
            _ = mongoDBSettings ?? throw new NullReferenceException(
                $"{nameof(mongoDBSettings)} is null");
            _ = mongoDBSettings.ConnectionString ?? throw new NullReferenceException(
                $"{nameof(mongoDBSettings.ConnectionString)} is null");
            _ = mongoDBSettings.DatabaseName ?? throw new NullReferenceException(
                $"{nameof(mongoDBSettings.DatabaseName)} is null");
            _ = mongoDBSettings.NotificationDatabaseName ?? throw new NullReferenceException(
                $"{nameof(mongoDBSettings.NotificationDatabaseName)} is null");
            builder.AppSettings.MongoDBSettings = mongoDBSettings;
            return builder;
        }
    }
}
