using BLL.Interfaces;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class EmailService : IEmailService
    {
        //private readonly IEmailConfiguration _emailConfiguration;
        private readonly IFluentEmail _singleEmail;
        private readonly ILogger<EmailService> _logger;
        public EmailService(/*IEmailConfiguration emailConfiguration,*/ IFluentEmail singleEmail, ILogger<EmailService> logger)
        {
            (/*_emailConfiguration, */_singleEmail, _logger) = (/*emailConfiguration,*/ singleEmail, logger);
        }
        public async Task SendAsync(string to, string header, string body)
        {
            if (string.IsNullOrEmpty(to))
                throw new ArgumentNullException($"{nameof(to)} is null or empty");
            if (string.IsNullOrEmpty(header))
                throw new ArgumentNullException($"{nameof(header)} is null or empty");
            if (string.IsNullOrEmpty(body))
                throw new ArgumentNullException($"{nameof(body)} is null or empty");
            _logger.LogDebug($"Send notification to: {to}, \r\nheader: {header}, body: {body}");
            #region Debug
            Random random = new Random();
            await Task.Delay(random.Next(500, 1000));
            if (random.Next(0, 9) <= 7)//20 percent failed send
                return;
            else
                throw new Exception();
            #endregion
            try
            {
                //email fluent
                var result = await _singleEmail
                    .To(to)
                    .Subject(header)
                    .Body(body)
                    .SendAsync(); //this will use the SmtpSender
                if (!result.Successful)
                {
                    foreach (var errore in result.ErrorMessages)
                    {
                        _logger.LogWarning(errore, $"Mailing to {to}");
                    }
                    throw new Exception("Message do not send");
                }

                //var from_ = new MailAddress(from, "VRealSoft");
                //var to_ = new MailAddress(to);
                //var msg = new MailMessage(from, to)
                //{
                //    Subject = header,
                //    Body = body,
                //    IsBodyHtml = false
                //};
                //using (SmtpClient smtp = new SmtpClient(_emailConfiguration.SmtpHost, _emailConfiguration.SmtpPort))
                //{
                //    smtp.Credentials = new NetworkCredential(from, _emailConfiguration.Password);
                //    smtp.EnableSsl = true;
                //    smtp.Send(msg);
                //}
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void Send(string to, string header, string body)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return Task.CompletedTask;
            //throw new NotImplementedException();
        }
    }
}
