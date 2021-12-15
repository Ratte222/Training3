using NotificationService.Interfaces;
using NotificationService.Services.Base;
using DAL_NS.EF;
using DAL_NS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Services
{
    public class NotificationService:BaseService<Notification, QueueSystemDbContext, string>, INotificationService
    {
        public NotificationService(QueueSystemDbContext context): base(context)
        { }

        public Notification[] GetForNotificationServiceSender()
        {
            return this.GetAll()
                        .Where(i => i.IsSend == false).Include(i => i.Exception)
                        .Include(i => i.Credentials).Include(i => i.MailSettings).AsNoTracking().ToArray();
        }

        public override Task DeleteRangeAsync(IEnumerable<Notification> items)
        {
            _context.ChangeTracker.Clear();
            return base.DeleteRangeAsync(items);
        }

        public override Task UpdateRangeAsync(IEnumerable<Notification> items)
        {
            _context.ChangeTracker.Clear();
            return base.UpdateRangeAsync(items);
        }
    }
}
