using DAL_NS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL_NS.EF
{
    public static class DbInitializer
    {
        public static void Initialize(QueueSystemDbContext context, List<Notification> notifications)
        {
            //initialize db from hard disk
            if (!context.Notifications.Any())
            {
                context.Notifications.AddRange(notifications);
                context.SaveChanges();
            }
        }
    }
}
