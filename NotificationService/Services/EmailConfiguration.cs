using NotificationService.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Services
{
    public class EmailConfiguration:IEmailConfiguration
    {
        public string SmtpEmail { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
    }
}
