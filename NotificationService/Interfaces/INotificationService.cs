using NotificationService.Interfaces.Base;
using DAL_NS.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Interfaces
{
    public interface INotificationService:IBaseService<Notification, string>
    {

    }
}
