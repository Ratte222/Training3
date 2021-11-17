﻿using DAL.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entity
{
    public class Credentials:BaseEntity<string>
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string NotificationId { get; set; }
        //public Notification Notification { get; set; }

        public Credentials()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
