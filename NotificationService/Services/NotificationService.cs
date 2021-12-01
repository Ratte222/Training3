using NotificationService.Interfaces;
using NotificationService.Services.BaseService;
using DAL.EF;
using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Services
{
    public class NotificationService:BaseService<Notification, QueueSystemDbContext, string>, INotificationService
    {
        public NotificationService(QueueSystemDbContext context): base(context)
        { }
    }
}
