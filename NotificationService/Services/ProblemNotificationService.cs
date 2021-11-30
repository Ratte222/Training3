using System.Threading;
using System.Threading.Tasks;
using NotificationService.Interfaces;
using DAL.Entity;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Services
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

        public async Task ExecuteAsync()
        {
            while (true)
            {
                try
                {                  
                    DateTime dateTime = DateTime.UtcNow;  
                    var problem_notifications = _notificationService.GetAll()
                        .Where(i=>((i.IsSend == false)&&(i.NumberOfAttemptToSent > _notificationSenderSettings.NumberOfAttemptToSentForPushToFile)
                        && ((dateTime - i.DateTimeCreate)> TimeSpan.FromMinutes(_notificationSenderSettings.TimeAfterCreateForPushToFile))
                        && i.DateTimeOfTheLastAttemptToSend.HasValue ? ((dateTime - i.DateTimeOfTheLastAttemptToSend.Value)<
                        TimeSpan.FromMinutes(_notificationSenderSettings.TimeOfTheLastAttemptToSendForPushToFile)) : false))
                        .Include(i=>i.Credentials).Include(i => i.Exception).AsEnumerable();
                    if(problem_notifications.Count() > 0)
                    {
                        await _notificationMongoRepository.AddRangeAsync(problem_notifications);
                        //with .AsNoTraking have errore: "The instance of entity type 'Notification' cannot be tracked because another instance with the same
                        //key value for {'Id'} is already being tracked. When attaching existing entities, ensure that only one
                        //entity instance with a given key value is attached. Consider using
                        //'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the conflicting key values."
                        await _notificationService.DeleteRangeAsync(problem_notifications);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Problem in {nameof(ProblemNotificationsService)}");
                    break;
                }
                await Task.Delay(TimeSpan.FromSeconds(_notificationSenderSettings.SleepTimeNotification), _cancellationToken);
                if (_cancellationToken.IsCancellationRequested)
                    break;
            }
            _logger.LogWarning($"{nameof(ProblemNotificationsService)} stoped!");
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