using NotificationService.Interfaces;
using NotificationService.Interfaces.NamedPipe;
using AuxiliaryLib.NamedPipe;
using DAL_NS.Entity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using NotificationService.Helpers;
using System.Threading.Tasks;

namespace NotificationService.Services.NamedPipe
{

    //https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-use-named-pipes-for-network-interprocess-communication?redirectedfrom=MSDN
    public class NamedPipeServerService: INamedPipeServerService
    {
        private int _numThreads = 0;
        //private readonly string pipeName_AddNotificationToQueue = "notificationServiceAddNotificationToQueue";
        private readonly ILogger<NamedPipeServerService> _logger;
        
        private readonly INotificationService _notificationService;
        private readonly INotificationMongoRepository _notificationMongoRepository;

        private AutoResetEvent waitHandler = new AutoResetEvent(true);
        private bool serviceWork = true;

        public NamedPipeServerService(ILogger<NamedPipeServerService> logger, INotificationService notificationService,
            INotificationMongoRepository notificationMongoRepository)
        {
            (_logger, _notificationService) = (logger, notificationService);
            _notificationMongoRepository = notificationMongoRepository;            
            
        }

        public async Task StartServiceAsync(IncomingDataForPipeServer incomingData, int numThreads = 5)
        {
            _numThreads = numThreads;
            int i;
            Thread[] servers = new Thread[_numThreads];

            _logger.LogDebug("\n*** Named pipe server stream with impersonation example ***\n");
            _logger.LogDebug("Waiting for client connect...\n");
            for (i = 0; i < _numThreads; i++)
            {
                servers[i] = new Thread(ServerThread);
                servers[i].Start(incomingData);
            }
            await Task.Delay(250);
            while (serviceWork)
            {
                for (int j = 0; j < _numThreads; j++)
                {
                    if (servers[j] != null)
                    {
                        if (servers[j].Join(250))
                        {
                            _logger.LogDebug("Server thread[{0}] finished.", servers[j].ManagedThreadId);
                            servers[j] = new Thread(ServerThread);
                            servers[j].Start(incomingData);
                            //servers[j] = null;
                            //i--;    // decrement the thread watch count
                        }
                    }
                }
            }
            for (int j = 0; j < _numThreads; j++)
            {
                if (servers[j] != null)
                {
                    servers[j].Abort(); 
                }
            }
            //return Task.CompletedTask;
        }

        private void ServerThread(object data)
        {
            IncomingDataForPipeServer incomingData = (IncomingDataForPipeServer)data;

            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream(incomingData.pipeName, PipeDirection.InOut, 30);
            int threadId = Thread.CurrentThread.ManagedThreadId;

            // Wait for a client to connect
            pipeServer.WaitForConnection();

            _logger.LogDebug("Client connected on thread[{0}].", threadId);
            try
            {
                StreamString ss = new StreamString(pipeServer);
                incomingData.func1?.Invoke(waitHandler, _notificationService, ss, _logger);
                incomingData.func2?.Invoke(_notificationMongoRepository, ss, _logger);
                incomingData.action1?.Invoke(waitHandler, _notificationService, _notificationMongoRepository,
                    ss, _logger);
            }
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
