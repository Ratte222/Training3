using System.Threading;
using System.Threading.Tasks;
using NotificationService.Interfaces;
using DAL_NS.Entity;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using FluentEmail.Core;

namespace NotificationService.Services
{
    public class ProblemNotificationsService:IProblemNotificationsService
    {
        /// <summary>
        /// email service to send exeption to the developer
        /// </summary>
        private readonly IFluentEmail _fluentEmail;
        private readonly ILogger<ProblemNotificationsService> _logger;
        private readonly INotificationMongoRepository _notificationMongoRepository;
        private readonly INotificationService _notificationService;
        private readonly INotificationSenderSettings _notificationSenderSettings;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public ProblemNotificationsService(ILogger<ProblemNotificationsService> logger,
            INotificationMongoRepository notificationMongoRepository, INotificationService notificationService,
            INotificationSenderSettings notificationSenderSettings, IFluentEmail fluentEmail)
        {
            (_logger, _notificationMongoRepository, _notificationService, _notificationSenderSettings)=
            (logger, notificationMongoRepository, notificationService, notificationSenderSettings);
            _fluentEmail = fluentEmail;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public async Task ExecuteAsync()
        {
            //while (true)
            //{
            try
            {                  
                var problem_notifications = _notificationService.GetAll()
                    .Where(i => i.IsSend == false)
                    .Where(i => i.NumberOfAttemptToSent > _notificationSenderSettings.NumberOfAttemptToSentForPushToFile)
                    .Where(i => i.DateTimeCreate < DateTime.UtcNow.AddMinutes(_notificationSenderSettings.TimeAfterCreateForPushToFile * -1))
                    //.Where(i => i.DateTimeOfTheLastAttemptToSend.HasValue ? ((dateTime - i.DateTimeOfTheLastAttemptToSend.Value) <
                    //TimeSpan.FromMinutes(_notificationSenderSettings.TimeOfTheLastAttemptToSendForPushToFile)) : false)
                    .Include(i=>i.Credentials).Include(i => i.Exception)
                    .AsNoTracking().AsEnumerable();
                if(problem_notifications.Count() > 0)
                {
                    var mongoHandle = await _notificationMongoRepository.StartSessionAsync();
                    await _notificationService.StartTransactionAsync();
                    try
                    {
                        await _notificationMongoRepository.AddRangeAsync(problem_notifications);
                        await _notificationService.DeleteRangeAsync(problem_notifications);
                        await mongoHandle.CommitTransactionAsync();
                        await _notificationService.CommitTransactionAsync();
                    }
                    catch(Exception ex)
                    {
                        await mongoHandle.AbortTransactionAsync();
                        await _notificationService.RollbackTransactionAsync();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Problem in {nameof(ProblemNotificationsService)}");
                if (_notificationSenderSettings.SendMailIfProblemNotificationServiceHasException)
                {
                    try
                    {
                        _fluentEmail.To(_notificationSenderSettings.DeveloperEmail)
                            .Subject($"Exception in {nameof(ProblemNotificationsService)}")
                            .Body(JsonConvert.SerializeObject(ex))
                            .Send();
                    }
                    catch { }
                }
                //if (_notificationSenderSettings.StopProblemNotificationServiceAfterException)
                //    break;
            }
            //await Task.Delay(TimeSpan.FromSeconds(_notificationSenderSettings.SleepTimeNotification), _cancellationToken);
            //if (_cancellationToken.IsCancellationRequested)
            //    break;
            //}
            //_logger.LogWarning($"{nameof(ProblemNotificationsService)} stoped!");
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