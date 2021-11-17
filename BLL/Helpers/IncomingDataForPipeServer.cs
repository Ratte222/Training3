using BLL.Interfaces;
using BLL.Services.NamedPipe;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using DAL.Entity;

namespace BLL.Helpers
{
    public struct IncomingDataForPipeServer
    {
        public string pipeName;
        public Func<AutoResetEvent, INotificationService, StreamString, ILogger, bool> func;
    }
    //public struct IncomingDataForPipeClient
    //{
    //    public string pipeName;
    //    public Func<Notification, StreamString, ILogger, bool> func;
    //}
}
