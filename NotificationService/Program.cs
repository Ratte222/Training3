using BLL.Interfaces;
using BLL.Interfaces.NamedPipe;
using BLL.Services.NamedPipe;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading;
using BLL.Helpers;

namespace NotificationService
{
    public class Program
    {
        public static ServiceProvider serviceProvider;
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            Startup.ConfigureService(ref serviceProvider);
            var logger = serviceProvider.GetService<ILogger<Program>>();
            try
            {
                INotificationServiceSender notificationServiceSender = serviceProvider.GetService<INotificationServiceSender>();
                IProblemNotificationsService problemNotificationsService = serviceProvider.GetService<IProblemNotificationsService>();
                INamedPipeServerService pipeServer_AddNotification = serviceProvider.GetService<INamedPipeServerService>();
                INamedPipeServerService pipeServer_Re_sendProblemNotifications = serviceProvider.GetService<INamedPipeServerService>();
                INamedPipeServerService pipeServer_CheckProblemNotification = serviceProvider.GetService<INamedPipeServerService>();
                INamedPipeServerService pipeServer_UpdateProblemNotification = serviceProvider.GetService<INamedPipeServerService>();
                notificationServiceSender.ExecuteAsync();
                problemNotificationsService.ExecuteAsync();
                pipeServer_AddNotification.StartServiceAsync(new IncomingDataForPipeServer(){
                    pipeName = "notificationServiceAddNotificationToQueue",
                    func1 = ServerFunctions.AddNotification
                }, 3);
                pipeServer_CheckProblemNotification.StartServiceAsync(new IncomingDataForPipeServer()
                {
                    pipeName = "notificationServiceCheckProblemMessage",
                    func2 = ServerFunctions.CheckProblemNotification
                }, 1);
                pipeServer_UpdateProblemNotification.StartServiceAsync(new IncomingDataForPipeServer()
                {
                    pipeName = "notificationServiceUpdateNotificationToQueue",
                    func2 = ServerFunctions.UpdateProblemNotification
                }, 1);
                pipeServer_Re_sendProblemNotifications.StartServiceAsync(new IncomingDataForPipeServer()
                {
                    pipeName = "notificationServiceRe_sendProblemNotifications",
                    action1 = ServerFunctions.Re_sendProblemNotifications
                }, 1);
                Console.WriteLine("Start read key");
                Console.ReadKey();
                pipeServer_AddNotification.Stop();
                pipeServer_CheckProblemNotification.Stop();
                pipeServer_UpdateProblemNotification.Stop();
                problemNotificationsService.Stop();
                notificationServiceSender.Stop();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "We have problems!");
            }
        }

        
    }

    
}