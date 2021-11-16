using System.Threading;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Entity;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;

namespace BLL.Services
{
    public class ProblemNotificationsService:IProblemNotificationsService
    {
        private readonly ILogger<ProblemNotificationsService> _logger;
        private readonly INotificationMongoRepository _notificationMongoRepository;
        private readonly INotificationService _notificationService;
        private readonly INotificationSenderSettings _notificationSenderSettings;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public ProblemNotificationsService(ILogger<ProblemNotificationsService> logger,
            INotificationMongoRepository notificationMongoRepository, INotificationService notificationService,
            INotificationSenderSettings notificationSenderSettings)
        {
            (_logger, _notificationMongoRepository, _notificationService, _notificationSenderSettings)=
            (logger, notificationMongoRepository, notificationService, notificationSenderSettings);
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public async Task Execute()
        {
            while (true)
            {
                try
                {                    
                    var problem_notifications = _notificationService.GetAll_Queryable()
                        .Where(i=>((i.IsSend == false)&&(i.NumberOfAttemptToSent > _notificationSenderSettings.NumberOfAttemptToSentForPushToFile)
                        && ((DateTime.UtcNow - i.DateTimeCreate)> TimeSpan.FromMinutes(_notificationSenderSettings.TimeAfterCreateForPushToFile))&&
                        ((DateTime.UtcNow - i.DateTimeOfTheLastAttemptToSend)<
                        TimeSpan.FromMinutes(_notificationSenderSettings.TimeOfTheLastAttemptToSendForPushToFile)))).AsEnumerable();
                    if(problem_notifications.Count() > 0)
                    {
                        await _notificationMongoRepository.AddRangeAsync(problem_notifications);
                        await _notificationService.DeleteRangeAsync(problem_notifications);
                    }
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