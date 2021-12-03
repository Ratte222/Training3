using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Interfaces
{
    public interface INotificationServiceSender:IDisposable
    {
        void Stop();
        Task ExecuteAsync();

    }
}
