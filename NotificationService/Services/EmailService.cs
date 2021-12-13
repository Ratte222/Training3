using NotificationService.Interfaces;
using DAL_NS.Entity;
//using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Services
{
    public class EmailService : IEmailService
    {
        //private readonly IEmailConfiguration _emailConfiguration;
        //private readonly IFluentEmail _singleEmail;
        private readonly ILogger<EmailService> _logger;
        
        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync(string to, string header, string body, Credentials credentials, MailSettings mailSettings)
        {
            _ = to ?? throw new ArgumentNullException($"{nameof(to)} is null or empty");
            _ = header ?? throw new ArgumentNullException($"{nameof(header)} is null or empty");
            _ = body ?? throw new ArgumentNullException($"{nameof(body)} is null or empty");
            _ = credentials ?? throw new ArgumentNullException($"{nameof(credentials)} is null");
            _ = credentials.Login ?? throw new ArgumentNullException($"{nameof(credentials.Login)} is null or empty");
            _ = credentials.Password ?? throw new ArgumentNullException($"{nameof(credentials.Password)} is null or empty");
            _ = credentials.SmtpHost ?? throw new ArgumentNullException($"{nameof(credentials.SmtpHost)} is null or empty");
            _ = mailSettings ?? throw new ArgumentNullException($"{nameof(mailSettings)} is null or empty");
            _ = mailSettings.DisplayName ?? throw new ArgumentNullException($"{nameof(mailSettings.DisplayName)} is null or empty");
            _logger.LogDebug($"Send notification to: {to}, \r\nheader: {header}, body: {body}");
            #if DEBUG
            Random random = new Random();
            await Task.Delay(random.Next(500, 1000));
            if (random.Next(0, 9) <= 7)//20 percent failed send
                return;
            else
                throw new Exception();
            #else
            try
            {
                var from_ = new MailAddress(credentials.Login, mailSettings.DisplayName/*"VRealSoft"*/);
                var to_ = new MailAddress(credentials.Login);
                var msg = new MailMessage(from_, to_)
                {
                    Subject = header,
                    Body = body,
                    IsBodyHtml = mailSettings.IsBodyHtml
                };
                using SmtpClient smtp = new SmtpClient(credentials.SmtpHost, credentials.SmtpPort);
                smtp.Credentials = new NetworkCredential(credentials.Login, credentials.Password);
                smtp.EnableSsl = mailSettings.EnableSsl;
                
                await smtp.SendMailAsync(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
#endif
        }

        //public async Task SendAsync(string to, string header, string body)
        //{
        //    if (string.IsNullOrEmpty(to))
        //        throw new ArgumentNullException($"{nameof(to)} is null or empty");
        //    if (string.IsNullOrEmpty(header))
        //        throw new ArgumentNullException($"{nameof(header)} is null or empty");
        //    if (string.IsNullOrEmpty(body))
        //        throw new ArgumentNullException($"{nameof(body)} is null or empty");
        //    _logger.LogDebug($"Send notification to: {to}, \r\nheader: {header}, body: {body}");
        //    #region Debug
        //    Random random = new Random();
        //    await Task.Delay(random.Next(500, 1000));
        //    if (random.Next(0, 9) <= 7)//20 percent failed send
        //        return;
        //    else
        //        throw new Exception();
        //    #endregion
        //    try
        //    {
        //        //email fluent
        //        var result = await _singleEmail
        //            .To(to)
        //            .Subject(header)
        //            .Body(body)
        //            .SendAsync(); //this will use the SmtpSender
        //        if (!result.Successful)
        //        {
        //            foreach (var errore in result.ErrorMessages)
        //            {
        //                _logger.LogWarning(errore, $"Mailing to {to}");
        //            }
        //            throw new Exception("Message do not send");
        //        }

        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

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
