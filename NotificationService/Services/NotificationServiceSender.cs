using BLL.Interfaces;
using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public class NotificationServiceSender:IDisposable
    {
        private readonly ILogger<NotificationServiceSender> _logger;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private AutoResetEvent waitHandler = new AutoResetEvent(true);
        public NotificationServiceSender(ILogger<NotificationServiceSender> logger, IEmailService emailService,
            INotificationService notificationService)
        {
            (_logger, _emailService, _notificationService) = (logger, emailService, notificationService);
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        //public void AddCancellationToken(CancellationTokenSource cancellationTokenSource)
        //{
        //    _cancellationTokenSource = cancellationTokenSource;
        //    _cancellationToken = cancellationTokenSource.Token;
        //}

        public void Stop()
        {            
            Dispose();
        }

        public async Task Execute()
        {
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
                    var sent_notifications = notifications.Where(i => i.IsSend == true).AsEnumerable();
                    await _notificationService.DeleteRangeAsync(sent_notifications);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Problems sending notifications");
                }
                await Task.Delay(TimeSpan.FromSeconds(20), _cancellationToken);
                if (_cancellationToken.IsCancellationRequested)
                    break;
            }
            _logger.LogWarning("NotificationServiceSender stoped");
        }

        private void Mailing(Notification notification)
        {
            if (notification is null)
                return;
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
            if (taskResult.Status == TaskStatus.RanToCompletion)
            {
                waitHandler.WaitOne();
                notification.IsSend = true;
                notification.DateTimeOfTheLastAttemptToSend = DateTime.UtcNow;
                waitHandler.Set();
            }
            else
            {
                waitHandler.WaitOne();
                _logger.LogWarning(taskResult.Exception, $"Notification id = {notification.Id}");
                notification.DateTimeOfTheLastAttemptToSend = DateTime.UtcNow;
                notification.NumberOfAttemptToSent++;
                waitHandler.Set();
            }
            //_context.Notifications.Update(notification);
        }

        #region Dispose
        private bool isDisposed = false;
        public void Dispose()
        {
            if(!isDisposed)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                isDisposed = true;
            }
        }
        #endregion
    }
}
