using System.Threading;
using BLL.Interfaces;
using System;
using Newtonsoft.Json;
using DAL.Entity;
using Microsoft.Extensions.Logging;

namespace BLL.Services.NamedPipe
{
    public static class ServerFunctions
    {
        public static bool AddNotification(AutoResetEvent waitHandler, INotificationService notificationService, StreamString ss,
            ILogger logger)
        {
            ss.WriteString("I am the one true server!");
            string JsonNotification = ss.ReadString();
            logger.LogDebug($"Recive message \"{JsonNotification}\"");
            //Console.WriteLine("Reading file: {0} on thread[{1}] as user: {2}.",
            //    filename, threadId, pipeServer.GetImpersonationUserName());
            Notification notification = JsonConvert.DeserializeObject<Notification>(JsonNotification);
            //notification.Id = Guid.NewGuid().ToString();
            notification.DateTimeCreate = DateTime.UtcNow;
            waitHandler.WaitOne();
            try
            {
                notificationService.Create(notification);
            }
            finally
            {
                waitHandler.Set();
            }
            ss.WriteString("Notification received successfully");
            return true;
        }
    }
}