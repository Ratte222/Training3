using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entity
{
    public class Credentials
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
    }
}
