using DAL.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entity
{
    public class NotificationException:BaseEntity<string>
    {
        public string Type { get; set; }
        public long? HResult { get; set; }
        public string Message { get; set; }
        public string NotificationId { get; set; }
        //public Notification Notification { get; set; }

        public NotificationException()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
