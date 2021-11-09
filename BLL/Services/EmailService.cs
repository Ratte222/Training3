using BLL.Interfaces;
using FluentEmail.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;
        private readonly IFluentEmail _singleEmail;
        public EmailService(IEmailConfiguration emailConfiguration, IFluentEmail singleEmail)
        {
            (_emailConfiguration, _singleEmail) = (emailConfiguration, singleEmail);
        }
        public Task Send(string From, string to, string heading, string body)
        {
            throw new NotImplementedException();
        }
    }
}
