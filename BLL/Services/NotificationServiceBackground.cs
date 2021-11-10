using AuxiliaryLib.BaseBackgroundService;
using BLL.Interfaces;
using DAL.EF;
using DAL.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Services
{
    //async tutuorials
    //https://www.youtube.com/watch?v=lHuyl_WTpME
    //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-simple-parallel-foreach-loop
    //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-parallel-for-loop-with-thread-local-variables
    //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-handle-exceptions-in-parallel-loops
    //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/exception-handling-task-parallel-library
    public class NotificationServiceBackground : BackgroundService
    {
        private QueueSystemDbContext _context;
        private IEmailService _emailService;
        private readonly IServiceProvider _services;
        private readonly ILogger<NotificationServiceBackground> _logger;
        public NotificationServiceBackground(IServiceProvider services,
            ILogger<NotificationServiceBackground> logger)
        {
            (_services, _logger) = (services, logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            var scope = _services.CreateScope();
            _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            _context = scope.ServiceProvider.GetRequiredService<QueueSystemDbContext>();
            //var builder = new ConfigurationBuilder().AddJsonFile("InitNotificationData.json");
            //var configuration = 
            //var _notifications = configuration.GetSection("Notifications").Get<List<Notification>>();
            //DbInitializer.Initialize(_context, _notifications);
            while (!stoppingToken.IsCancellationRequested)
            {
                var notifications = _context.Notifications.Where(i=>i.IsSend == false).ToList();
                Parallel.ForEach(notifications, notification =>
                {
                    switch (notification.TypeNotification)
                    {
                        case TypeNotification.Email:
                            Mailing(notification);
                            break;
                    };
                });
                //foreach(var notification in notifications)
                //{
                //    switch (notification.TypeNotification)
                //    {
                //        case TypeNotification.Email:
                //            Mailing(notification);
                //            break;
                //    };
                //}
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }

        private void Mailing(Notification notification)
        {
            //Task taskResult = _emailService.Send(notification.Sender,
            //    notification.Recipient, notification.Heading, notification.MessageBody);
            Task taskResult = _emailService.SendAsync(
                notification.Recipient, notification.Header, notification.MessageBody);
            try//just in case
            {
                taskResult.Wait();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Notification id = {notification.Id}");
            }
            if(taskResult.Status == TaskStatus.RanToCompletion)
            {
                notification.IsSend = true;
                notification.DateTimeOfTheLastAttemptToSend = DateTime.UtcNow;
            }
            else
            {
                _logger.LogWarning(taskResult.Exception, $"Notification id = {notification.Id}");
                notification.DateTimeOfTheLastAttemptToSend = DateTime.UtcNow;
                notification.NumberOfAttemptToSent++;
            }
        }
        private async Task SendTelegram(Notification notification)
        {

        }
    }
}
