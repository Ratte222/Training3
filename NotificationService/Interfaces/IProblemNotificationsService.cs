using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace NotificationService.Interfaces
{
    public interface IProblemNotificationsService:IDisposable
    {
        void Stop();
        Task ExecuteAsync();
        Task AddNotificationInQueue(int take);
        void SetServiceProvider(ServiceProvider serviceProvider);
    }
}