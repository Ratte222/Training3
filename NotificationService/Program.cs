using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.Services;
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
                var notificationServiceSender = serviceProvider.GetService<NotificationServiceSender>();
                notificationServiceSender.Execute();
                PipeServerService pipeServerService = serviceProvider.GetService<PipeServerService>();
                pipeServerService.StartService();
                Console.WriteLine("Start read key");
                Console.ReadKey();
                notificationServiceSender.Stop();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "We have problems!");
            }
        }

        
    }

    
}