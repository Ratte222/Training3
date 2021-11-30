using NotificationService.Interfaces;
using NotificationService.Services.NamedPipe;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using DAL.Entity;

namespace NotificationService.Helpers
{
    public struct IncomingDataForPipeServer
    {
        public string pipeName;
        public Func<AutoResetEvent, INotificationService, StreamString, ILogger, bool> func1;
        public Func<INotificationMongoRepository, StreamString, ILogger, bool> func2;
        public Action<AutoResetEvent, INotificationService, INotificationMongoRepository, 
            StreamString, ILogger> action1;

    }
    //public struct IncomingDataForPipeClient
    //{
    //    public string pipeName;
    //    public Func<Notification, StreamString, ILogger, bool> func;
    //}
}
