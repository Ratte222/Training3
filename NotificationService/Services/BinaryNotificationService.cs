using DAL_NS.Entity;
using NotificationService.Interfaces;
using NotificationService.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NotificationService.Services
{
    class BinaryNotificationService : FileRepoBase<Notification>, INotificationService
    {
        
        public BinaryNotificationService(IFileProviderService<Notification> fileProviderService)
            : base(fileProviderService) { }
        public Notification[] GetForNotificationServiceSender()
        {
            return this.GetAll_Enumerable().ToArray();
        }
    }
}
