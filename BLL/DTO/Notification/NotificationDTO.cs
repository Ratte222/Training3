using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO.Notification
{
    public class NotificationDTO
    {
        public string Id { get; set; }
        public TypeNotification TypeNotification { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public string Header { get; set; }
        public string MessageBody { get; set; }
        public bool IsSend { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public DateTime? DateTimeOfTheLastAttemptToSend { get; set; }
        public int NumberOfAttemptToSent { get; set; }

        public Credentials Credentials { get; set; }
        public NotificationException Exception { get; set; }
    }
}
