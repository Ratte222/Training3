using NotificationService.Interfaces;
using DAL_NS.Entity;
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
    public class NotificationServiceSender: INotificationServiceSender
    {
        private readonly ILogger<NotificationServiceSender> _logger;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly INotificationSenderSettings _notificationSenderSettings;
        
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private AutoResetEvent waitHandler = new AutoResetEvent(true);

        public NotificationServiceSender(ILogger<NotificationServiceSender> logger, IEmailService emailService,
            INotificationService notificationService, INotificationSenderSettings notificationSenderSettings)
        {
            (_logger, _emailService, _notificationService, _notificationSenderSettings) = 
                (logger, emailService, notificationService, notificationSenderSettings);
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        //public void AddCancellationToken(CancellationTokenSource cancellationTokenSource)
        //{
        //    _cancellationTokenSource = cancellationTokenSource;
        //    _cancellationToken = cancellationTokenSource.Token;
        //}

        public async Task ExecuteAsync()
        {
            while (true)
            {
                try
                {
                    var notifications = _notificationService.GetAll()//.AsNoTracking()
                        .Where(i => i.IsSend == false).Include(i=>i.Credentials);
                    //.ToList();
                    Parallel.ForEach(notifications, notification =>
                    {
                        switch (notification.TypeNotification)
                        {
                            case TypeNotification.Email:
                                Mailing(notification);
                                break;
                            case TypeNotification.Telegram:
                                SendViaTelegram(notification);
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
                await Task.Delay(TimeSpan.FromSeconds(_notificationSenderSettings.SleepTimeNotification), _cancellationToken);
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
                notification.Recipient, notification.Header, notification.MessageBody, notification.Credentials);
            try//just in case
            {
                taskResult.Wait();
            }            
            catch (Exception ex)//SmtpException, ObjectDisposedException, InvalidOperationException, ArgumentNullException
            {
                _logger.LogInformation(ex, $"Notification id = {notification.Id}");                
            }
            if (taskResult.Status == TaskStatus.RanToCompletion)
            {
                PositiveResultSend(notification);                
            }
            else
            {
                NegativeResultSend(notification, taskResult.Exception);                
            }
            //_context.Notifications.Update(notification);
        }

        private void SendViaTelegram(Notification notification)
        {
            var exception = new NotImplementedException();
            NegativeResultSend(notification, exception);
        }

        private void PositiveResultSend(Notification notification)
        {
            waitHandler.WaitOne();
            _logger.LogDebug($"Notification sent, id = {notification.Id}");
            notification.IsSend = true;
            notification.DateTimeOfTheLastAttemptToSend = DateTime.UtcNow;
            waitHandler.Set();
        }

        private void NegativeResultSend(Notification notification, Exception exception)
        {
            waitHandler.WaitOne();
            _logger.LogDebug($"Notification not sent, id = {notification.Id}");
            _logger.LogInformation(exception, $"Notification id = {notification.Id}");
            notification.Exception.Type = exception.GetType().FullName;
            notification.Exception.HResult = exception.HResult;
            notification.Exception.Message = exception.Message;
            notification.DateTimeOfTheLastAttemptToSend = DateTime.UtcNow;
            notification.NumberOfAttemptToSent++;
            waitHandler.Set();
        }

        #region Dispose
        public void Stop()
        {            
            Dispose();
        }
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
