using DAL.EF;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using BLL.Services;
using BLL.Interfaces;
using System.Net.Mail;
using System.Net;
using BLL.Interfaces.NamedPipe;
using BLL.Services.NamedPipe;
using BLL.Helpers;
using MongoDB.Bson;

namespace NotificationService
{
    public static class Startup
    {
        private static string AppSettingsFileName = "appsettings.json";
        public static void ConfigureService(ref ServiceProvider servicesProvider)
        {
            //setup our DI
            var services = new ServiceCollection();
            services.AddLogging(configure => configure.AddSerilog());
            #region DatabaseConfiguration
            services.AddDbContext<QueueSystemDbContext>(options => options.UseInMemoryDatabase("Notification"), 
                ServiceLifetime.Transient);
            #endregion
            #region Configuration from appsettings
            if (!File.Exists(AppSettingsFileName))
            {
                throw new Exception($"file {AppSettingsFileName} does not exist");
            }
            string content = File.ReadAllText(AppSettingsFileName);
            //TODO: use Microsoft.Extensions.Configuration
            JToken parsedJson = JObject.Parse(content)["EmailConfiguration"];
            EmailConfiguration emailConfiguration = parsedJson.ToObject<EmailConfiguration>();
            services.AddSingleton<IEmailConfiguration>(emailConfiguration);

            parsedJson = JObject.Parse(content)["NotificationSenderSettings"];
            NotificationSenderSettings notificationSenderSettings = parsedJson.ToObject<NotificationSenderSettings>();
            services.AddSingleton<INotificationSenderSettings>(notificationSenderSettings);

            parsedJson = JObject.Parse(content)["MongoDBSettings"];
            MongoDBSettings mongoDBSettings = parsedJson.ToObject<MongoDBSettings>();
            services.AddSingleton<MongoDBSettings>(mongoDBSettings);
            #endregion

            //BsonDefaults.MaxSerializationDepth = 2;
            
            //#region FluentEmail_Smtp
            //SmtpClient smtp = new SmtpClient
            //{
            //    //The address of the SMTP server (I'll take mailbox 126 as an example, which can be set according to the specific mailbox you use)
            //    Host = emailConfiguration.SmtpHost,
            //    Port = emailConfiguration.SmtpPort,
            //    UseDefaultCredentials = true,
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    //Enter the user name and password of your sending SMTP server here
            //    Credentials = new NetworkCredential(emailConfiguration.SmtpEmail, emailConfiguration.SmtpPassword)
            //};
            //services
            //    .AddFluentEmail(emailConfiguration.SmtpEmail)
            //    .AddSmtpSender(smtp); //configure host and port
            //#endregion

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<INotificationService, BLL.Services.NotificationService>();
            services.AddTransient<INamedPipeServerService, NamedPipeServerService>();
            services.AddScoped<INotificationServiceSender, NotificationServiceSender>();
            services.AddScoped<INotificationMongoRepository, NotificationMongoRepository>();
            services.AddScoped<IProblemNotificationsService, ProblemNotificationsService>();
            servicesProvider = services.BuildServiceProvider();
            
        }
    }
}
