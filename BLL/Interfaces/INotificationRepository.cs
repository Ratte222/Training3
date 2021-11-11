﻿using DAL.Entity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface INotificationRepository
    {
        IQueryable<Notification> GetQueryable();
        IEnumerable<Notification> Find(FilterDefinition<Notification> filterDefinition);
        Task AddRangeAsync(IEnumerable<Notification> notifications);
        Task ReplaceManyByIdAsync(IEnumerable<Notification> notifications);
    }
}
