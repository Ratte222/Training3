using BLL.Interfaces.NamedPipe;
using DAL.Entity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace BLL.Services.NamedPipe
{
    public class NamedPipeClientService:INamedPipeClientService
    {
        private readonly ILogger<NamedPipeClientService> _logger;
        private readonly string pipeName = "notificationServiceAddNotificationToQueue";

        public NamedPipeClientService(ILogger<NamedPipeClientService> logger)
        {
            (_logger) = (logger);
        }

        public bool SendNotification(Notification notification)
        {
            bool result = false;
            var pipeClient =
                         new NamedPipeClientStream(".", pipeName,
                             PipeDirection.InOut, PipeOptions.None,
                             TokenImpersonationLevel.Impersonation);

            _logger.LogDebug("Connecting to server...\n");
            pipeClient.Connect();

            var ss = new StreamString(pipeClient);
            // Validate the server's signature string.
            if (ss.ReadString() == "I am the one true server!")
            {
                string jsonString = JsonConvert.SerializeObject(notification);
                ss.WriteString(jsonString);
                try 
                {
                    var res = ss.ReadString();
                    if (res.Equals("Notification received successfully"))//all good
                    {
                        _logger.LogInformation(res);
                        result = true;
                    }
                    else//has problem
                    {
                        _logger.LogWarning("Notification not processed by notification server");
                    }
                }
                catch(OverflowException ex)//most likely problems with the notification server
                {
                    _logger.LogWarning("Notification not processed by notification server");
                }
                
            }
            else
            {
                _logger.LogInformation("Server could not be verified.");
            }
            pipeClient.Close();
            return result;
        }
    }
}


