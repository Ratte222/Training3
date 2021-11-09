﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    public interface IEmailConfiguration
    {
        string SmtpEmail { get; set; }
        string SmtpPassword { get; set; }
        string SmtpHost { get; set; }
        int SmtpPort { get; set; }
    }
}
