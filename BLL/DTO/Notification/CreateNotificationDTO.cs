using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO.Notification
{
    public class CreateNotificationDTO
    {
        public TypeNotification TypeNotification { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public string Header { get; set; }
        public string MessageBody { get; set; }
    }
}
