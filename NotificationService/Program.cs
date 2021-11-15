using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.Services;
using Serilog;
using System;

namespace NotificationService
{
    public class Program
    {
        public static ServiceProvider serviceProvider;
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            Startup.ConfigureService(ref serviceProvider);
            var logger = serviceProvider.GetService<ILogger<Program>>();
            try
            {
                PipeServerService pipeServerService = serviceProvider.GetService<PipeServerService>();
                pipeServerService.StartService();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "We have problems!");
            }
        }

        
    }

    
}