using NotificationService.Interfaces;
using NotificationService.Interfaces.NamedPipe;
using NotificationService.Services.NamedPipe;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading;
using NotificationService.Helpers;

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
            Startup startup = new Startup();
            startup.ConfigureService(ref serviceProvider);
            var logger = serviceProvider.GetService<ILogger<Program>>();
            try
            {
                INotificationServiceSender notificationServiceSender = serviceProvider.GetService<INotificationServiceSender>();
                notificationServiceSender.SetServiceProvider(serviceProvider);
                IProblemNotificationsService problemNotificationsService = serviceProvider.GetService<IProblemNotificationsService>();
                problemNotificationsService.SetServiceProvider(serviceProvider);
                INamedPipeServerService pipeServer_AddNotification = serviceProvider.GetService<INamedPipeServerService>();
                INamedPipeServerService pipeServer_Re_sendProblemNotifications = serviceProvider.GetService<INamedPipeServerService>();
                INamedPipeServerService pipeServer_CheckProblemNotification = serviceProvider.GetService<INamedPipeServerService>();
                INamedPipeServerService pipeServer_UpdateProblemNotification = serviceProvider.GetService<INamedPipeServerService>();
                ServerFunctions.Re_sendProblemNotificationsEvent += problemNotificationsService.AddNotificationInQueue;
                notificationServiceSender.NeedCheckProblemNotification += problemNotificationsService.ExecuteAsync;
                notificationServiceSender.ExecuteAsync();
                //problemNotificationsService.ExecuteAsync();
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