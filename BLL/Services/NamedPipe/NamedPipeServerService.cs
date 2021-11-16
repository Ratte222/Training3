using BLL.Interfaces;
using BLL.Interfaces.NamedPipe;
using BLL.Services.NamedPipe;
using DAL.Entity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace BLL.Services.NamedPipe
{

    //https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-use-named-pipes-for-network-interprocess-communication?redirectedfrom=MSDN
    public class NamedPipeServerService: INamedPipeServerService
    {
        private readonly int numThreads = 10;
        private readonly string pipeName_AddNotificationToQueue = "notificationServiceAddNotificationToQueue";
        private readonly ILogger<NamedPipeServerService> _logger;
        private readonly INotificationService _notificationService;

        private bool serviceWork = true;

        public NamedPipeServerService(ILogger<NamedPipeServerService> logger, INotificationService notificationService)
        {
            (_logger, _notificationService) = (logger, notificationService);
        }

        public void StartService()
        {
            int i;
            Thread[] servers = new Thread[numThreads];

            _logger.LogDebug("\n*** Named pipe server stream with impersonation example ***\n");
            _logger.LogDebug("Waiting for client connect...\n");
            for (i = 0; i < numThreads; i++)
            {
                servers[i] = new Thread(ServerThread);
                servers[i].Start();
            }
            Thread.Sleep(250);
            while (serviceWork)
            {
                for (int j = 0; j < numThreads; j++)
                {
                    if (servers[j] != null)
                    {
                        if (servers[j].Join(250))
                        {
                            _logger.LogDebug("Server thread[{0}] finished.", servers[j].ManagedThreadId);
                            servers[j] = new Thread(ServerThread);
                            servers[j].Start();
                            //servers[j] = null;
                            //i--;    // decrement the thread watch count
                        }
                    }
                }
            }
            for (int j = 0; j < numThreads; j++)
            {
                if (servers[j] != null)
                {
                    servers[j].Abort(); 
                }
            }
        }

        private void ServerThread(object data)
        {
            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream(pipeName_AddNotificationToQueue, PipeDirection.InOut, numThreads);
            int threadId = Thread.CurrentThread.ManagedThreadId;

            // Wait for a client to connect
            pipeServer.WaitForConnection();

            _logger.LogDebug("Client connected on thread[{0}].", threadId);
            try
            {
                StreamString ss = new StreamString(pipeServer);

                ss.WriteString("I am the one true server!");
                string JsonNotification = ss.ReadString();
                _logger.LogDebug($"Recive message \"{JsonNotification}\"");
                //Console.WriteLine("Reading file: {0} on thread[{1}] as user: {2}.",
                //    filename, threadId, pipeServer.GetImpersonationUserName());
                Notification notification = JsonConvert.DeserializeObject<Notification>(JsonNotification);
                notification.Id = Guid.NewGuid().ToString();
                _notificationService.Create(notification);
                ss.WriteString("Notification received successfully");
            }
            // Catch the IOException that is raised if the pipe is broken
            // or disconnected.
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "");
            }
            pipeServer.Close();
        }
        
        public void Stop()
        {
            serviceWork = false;
        }

    }
    
}
