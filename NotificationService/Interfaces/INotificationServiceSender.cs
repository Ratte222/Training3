using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Interfaces
{
    public interface INotificationServiceSender:IDisposable
    {
        event Func<Task> NeedCheckProblemNotification;
        void Stop();
        Task ExecuteAsync();
        void SetServiceProvider(ServiceProvider serviceProvider);
    }
}
