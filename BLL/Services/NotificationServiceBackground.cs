using AuxiliaryLib.BaseBackgroundService;
using BLL.Helpers;
using BLL.Interfaces;
using DAL.EF;
using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
        //private QueueSystemDbContext _context;
        private IEmailService _emailService;
        private INotificationMongoRepository _notificationMongoRepository;
        private INotificationService _notificationService;
        private readonly IServiceProvider _services;
        private readonly ILogger<NotificationServiceBackground> _logger;
        
        public NotificationServiceBackground(IServiceProvider services,
            ILogger<NotificationServiceBackground> logger)
        {
            (_services, _logger) = (services, logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            long lastId = 0;
            IServiceScope scope = null;
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                scope = _services.CreateScope();
                _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                //_context = scope.ServiceProvider.GetRequiredService<QueueSystemDbContext>();
                _notificationMongoRepository = scope.ServiceProvider.GetRequiredService<INotificationMongoRepository>();
                _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                

                //var temp = _notificationMongoRepository.Find(Builders<Notification>.Filter.Eq(nf => nf.IsSend, false));
                var initNotificationsFromMongo = _notificationMongoRepository.GetQueryable().AsEnumerable();
                await _notificationService.CreateRangeAsync(initNotificationsFromMongo);
                lastId = (_notificationService.GetAll_Queryable().Count() > 0) ? 
                    _notificationService.GetAll_Queryable().Last().Id : 0;
                InitDataFromJson();
                
                
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex, "The notification service failed to initialize");
                return;
            }
            
            while (true)
            {
                try
                {
                    var notifications = _notificationService.GetAll()//.AsNoTracking()
                        .Where(i => i.IsSend == false);
                        //.ToList();
                    Parallel.ForEach(notifications, notification =>
                    {
                        switch (notification.TypeNotification)
                        {
                            case TypeNotification.Email:
                                Mailing(notification);
                                break;
                        };
                    });
                    await _notificationService.UpdateRangeAsync(notifications);
                    
                    //foreach(var notification in notifications)
                    //{
                    //    switch (notification.TypeNotification)
                    //    {
                    //        case TypeNotification.Email:
                    //            Mailing(notification);
                    //            break;
                    //    };
                    //}
                    if (lastId < _notificationService.GetAll_Queryable().Last().Id)
                    {
                        var notifications_ = _notificationService.GetAll_Queryable().Where(n => n.Id > lastId)
                            .OrderBy(i=>i.Id).AsEnumerable();                        
                        await _notificationMongoRepository.AddRangeAsync(notifications_);
                        lastId = notifications_.Last().Id;
                    }
                    var sent_notifications = notifications.Where(i => i.IsSend == true).AsEnumerable();                    
                    await _notificationMongoRepository.ReplaceManyByIdAsync(sent_notifications);
                    //await _notificationService.DeleteRangeAsync(temp2);
                }
                catch(Exception ex)
                {
                    _logger.LogWarning(ex, "Problems sending notifications");
                }
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                if (stoppingToken.IsCancellationRequested)
                    break;
            }
            scope?.Dispose();
        }

        private void InitDataFromJson()
        {
            string fileName = "InitNotificationData.json";
            if(File.Exists(fileName) && !_notificationService.GetAll_Queryable().Any())
            {
                string content = "";
                using(StreamReader sr = new StreamReader(fileName))
                {
                    content = sr.ReadToEnd();
                }
                JToken parsedJson = JObject.Parse(content)["Notifications"];
                List<Notification> notifications = parsedJson.ToObject<List<Notification>>();
                notifications.ForEach(n => n.DateTimeCreate = DateTime.UtcNow);
                _notificationService.CreateRange(notifications);
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
            //_context.Notifications.Update(notification);
        }
        private async Task SendTelegram(Notification notification)
        {

        }
    }
}
