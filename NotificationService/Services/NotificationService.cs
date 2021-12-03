using NotificationService.Interfaces;
using NotificationService.Services.BaseService;
using DAL_NS.EF;
using DAL_NS.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Services
{
    public class NotificationService:BaseService<Notification, QueueSystemDbContext, string>, INotificationService
    {
        public NotificationService(QueueSystemDbContext context): base(context)
        { }

        public override Task DeleteRangeAsync(IEnumerable<Notification> items)
        {
            //_context.ChangeTracker.Clear();
            return base.DeleteRangeAsync(items);
        }

        
    }
}
