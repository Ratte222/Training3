using AuxiliaryLib.Helpers;
using AuxiliaryLib.NamedPipe;
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
        private readonly string pipeName_AddNotificationToQueue = "notificationServiceAddNotificationToQueue";
        private readonly string pipeName_UpdateNotificationToQueue = "notificationServiceUpdateNotificationToQueue";
        private readonly string pipeName_Re_sendProblemNotifications = "notificationServiceRe_sendProblemNotifications";
        private readonly string pipeName_CheckProblemMessage = "notificationServiceCheckProblemMessage";

        public NamedPipeClientService(ILogger<NamedPipeClientService> logger)
        {
            (_logger) = (logger);
        }

        public bool SendNotification(Notification notification)
        {
            bool result = false;
            var pipeClient =
                         new NamedPipeClientStream(".", pipeName_AddNotificationToQueue,
                             PipeDirection.InOut, PipeOptions.None,
                             TokenImpersonationLevel.Impersonation);

            _logger.LogDebug("Connecting to server...\n");
            pipeClient.Connect();

            var ss = new StreamString(pipeClient);
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
                catch (OverflowException ex)//most likely problems with the notification server
                {
                    _logger.LogWarning(ex, "Notification not processed by notification server");
                }

            }
            else
            {
                _logger.LogInformation("Server could not be verified.");
            }

            pipeClient.Close();
            return result;
        }

        public bool Re_sendProblemNotifications()
        {
            bool result = false;
            var pipeClient =
                         new NamedPipeClientStream(".", pipeName_Re_sendProblemNotifications,
                             PipeDirection.InOut, PipeOptions.None,
                             TokenImpersonationLevel.Impersonation);

            _logger.LogDebug("Connecting to server...\n");
            pipeClient.Connect();

            var ss = new StreamString(pipeClient);
            if (ss.ReadString() == "I am the one true server!")
            {
                ss.WriteString("Re_send");
                var res = ss.ReadString();
                if (res.Equals(ServiceAnswers.AnswerOk))//all good
                {
                    _logger.LogInformation(res);
                    result = true;
                }                
            }
            else
            {
                _logger.LogInformation("Server could not be verified.");
            }

            pipeClient.Close();
            return result;
        }

        public bool UpdateProblemNotification(Notification notification)
        {
            bool result = false;
            var pipeClient =
                         new NamedPipeClientStream(".", pipeName_UpdateNotificationToQueue,
                             PipeDirection.InOut, PipeOptions.None,
                             TokenImpersonationLevel.Impersonation);

            _logger.LogDebug("Connecting to server...\n");
            pipeClient.Connect();

            var ss = new StreamString(pipeClient);
            if (ss.ReadString() == "I am the one true server!")
            {
                string jsonString = JsonConvert.SerializeObject(notification);
                ss.WriteString(jsonString);
                try
                {
                    var res = ss.ReadString();
                    if (res.Equals("Notification updated successfully"))//all good
                    {
                        _logger.LogInformation(res);
                        result = true;
                    }
                    else//has problem
                    {
                        _logger.LogWarning("Notification not processed by notification server");
                    }
                }
                catch (OverflowException ex)//most likely problems with the notification server
                {
                    _logger.LogWarning(ex, "Notification not processed by notification server");
                }

            }
            else
            {
                _logger.LogInformation("Server could not be verified.");
            }

            pipeClient.Close();
            return result;
        }

        public PageResponse<Notification> CheckProblemNotification(int? pageLength = null, int? pageNumber = null)
        {
            PageResponse<Notification> result = new PageResponse<Notification>(pageLength, pageNumber);
            var pipeClient =
                         new NamedPipeClientStream(".", pipeName_CheckProblemMessage,
                             PipeDirection.InOut, PipeOptions.None,
                             TokenImpersonationLevel.Impersonation);

            _logger.LogDebug("Connecting to server...\n");
            pipeClient.Connect();

            var ss = new StreamString(pipeClient);
            if (ss.ReadString() == "I am the one true server!")
            {
                string paginateDataJson = JsonConvert.SerializeObject(new {
                    PageLength = result.PageLength,
                    PageNumber = result.PageNumber });
                ss.WriteString(paginateDataJson);
                string problemNotificationsJson = ss.ReadString();
                if (!problemNotificationsJson.Equals(ServiceAnswers.AnswerNotFound))
                {
                    result = JsonConvert.DeserializeObject<PageResponse<Notification>>(problemNotificationsJson);
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


