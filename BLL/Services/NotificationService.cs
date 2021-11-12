using BLL.Interfaces;
using BLL.Services.BaseService;
using DAL.EF;
using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services
{
    public class NotificationService:BaseService<Notification, QueueSystemDbContext>, INotificationService
    {
        public NotificationService(QueueSystemDbContext context): base(context)
        { }
    }
}
