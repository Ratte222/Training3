using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            notificationServiceBuilder.NotificationSenderSettings.QueueDatabaseType = QueueDatabaseType.MySQL;
            notificationServiceBuilder.AppSettings.ConnectionStrings.MySQL = connectionString;
            return notificationServiceBuilder;
        }
    }
}
