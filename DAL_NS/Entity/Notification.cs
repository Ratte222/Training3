using DAL_NS.Entity.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_NS.Entity
{
    public class Notification:BaseEntity<string>
    {
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
        public NotificationException Exception { get; set; } = new NotificationException();

        public Notification()
        {
            //Id = Guid.NewGuid().ToString();
        }
    }

    public enum TypeNotification
    {
        Email = 0,
        Telegram,
        WhatsApp
    }
}
