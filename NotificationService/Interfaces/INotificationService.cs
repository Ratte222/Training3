﻿using NotificationService.Interfaces.Base;
using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Interfaces
{
    public interface INotificationService:IBaseService<Notification, string>
    {

    }
}
