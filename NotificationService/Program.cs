using BLL.Interfaces;
using BLL.Interfaces.NamedPipe;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading;

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
                var notificationServiceSender = serviceProvider.GetService<INotificationServiceSender>();
                notificationServiceSender.Execute();
                INamedPipeServerService pipeServerService = serviceProvider.GetService<INamedPipeServerService>();
                pipeServerService.StartService();
                Console.WriteLine("Start read key");
                Console.ReadKey();
                pipeServerService.Stop();
                notificationServiceSender.Stop();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "We have problems!");
            }
        }

        
    }

    
}