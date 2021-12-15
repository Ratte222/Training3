using DAL_NS.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_NS.Entity
{
    [Serializable]
    public class MailSettings:BaseEntity<string>
    {
        public string DisplayName { get; set; }
        public bool EnableSsl { get; set; }
        public string NotificationId { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}
