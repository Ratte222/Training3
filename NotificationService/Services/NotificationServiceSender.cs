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
using FluentEmail.Core;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Services
{
    public class NotificationServiceSender: INotificationServiceSender
    {
        public event Func<Task> NeedCheckProblemNotification;

        private readonly ILogger<NotificationServiceSender> _logger;
        private readonly IEmailService _emailService;

        /// <summary>
        /// email service to send exeption to the developer
        /// </summary>
        private readonly IFluentEmail _fluentEmail;
        private readonly INotificationService _notificationService;
        private readonly INotificationSenderSettings _notificationSenderSettings;
        
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private AutoResetEvent waitHandler = new AutoResetEvent(true);

        public NotificationServiceSender(ILogger<NotificationServiceSender> logger, IEmailService emailService,
            INotificationService notificationService, INotificationSenderSettings notificationSenderSettings,
            IFluentEmail fluentEmail)
        {
            (_logger, _emailService,  _notificationSenderSettings) = 
                (logger, emailService,  notificationSenderSettings);
            _notificationService = notificationService;
            _fluentEmail = fluentEmail;
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
                    var notifications = _notificationService.GetAll()
                        .Where(i => i.IsSend == false).Include(i=>i.Exception)
                        .Include(i => i.Credentials).Include(i=>i.MailSettings).AsNoTracking().ToArray();
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

                    //for(int i = 0; i< notifications.Count(); i++)
                    //{
                    //    switch (notifications[i].TypeNotification)
                    //    {
                    //        case TypeNotification.Email:
                    //            Mailing(notifications[i]);
                    //            break;
                    //        case TypeNotification.Telegram:
                    //            SendViaTelegram(notifications[i]);
                    //            break;

                    //    };
                    //}
                    //await _notificationService.UpdateRangeAsync(notifications);
                    var sent_notifications = notifications.Where(i => i.IsSend == true).ToArray();

                    //var sent_notifications = _notificationService.GetAll_Queryable()
                    //    .Where(i => i.IsSend == true).ToArray();
                    //var temp = _notificationService.GetAll_Queryable().Include(i => i.Credentials);
                    await _notificationService.UpdateRangeAsync(notifications.Except(sent_notifications));
                    await _notificationService.DeleteRangeAsync(sent_notifications);
                    await NeedCheckProblemNotification?.Invoke();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Problems sending notifications");
                    if(_notificationSenderSettings.SendMailIfNotificationServiceSenderHasException)
                    {
                        try
                        {
                            _fluentEmail.To(_notificationSenderSettings.DeveloperEmail)
                                .Subject($"Exception in {nameof(NotificationServiceSender)}")
                                .Body(JsonConvert.SerializeObject(ex))
                                .Send();
                        }
                        catch { }
                    }
                    if (_notificationSenderSettings.StopNotificationServiceSenderAfterException)
                        break;
                }
                //finally
                //{
                //    notificationService.Dispose();
                //}
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
                notification.Recipient, notification.Header, notification.MessageBody, 
                notification.Credentials, notification.MailSettings);
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
