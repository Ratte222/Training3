#define UseMySQL
using DAL_NS.EF;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using NotificationService.Services;
using NotificationService.Interfaces;
using System.Net.Mail;
using System.Net;
using NotificationService.Interfaces.NamedPipe;
using AuxiliaryLib.NamedPipe;
using NotificationService.Helpers;
using MongoDB.Bson;
using NotificationService.Services.NamedPipe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using NotificationService.Constants;

namespace NotificationService
{
    public class Startup
    {
        private const string AppSettingsFileName = "appsettings.json";
        private const string ConfigureNotificationServiceFileName = "ConfigureNotificationService.json";

        private readonly IConfiguration Configuration;

        public Startup()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile(AppSettingsFileName).AddJsonFile(ConfigureNotificationServiceFileName);
            Configuration = builder.Build();
        }

        public void ConfigureService(ref ServiceProvider servicesProvider)
        {
            //setup our DI
            var services = new ServiceCollection();
            services.AddLogging(configure => configure.AddSerilog());
            #region Configuration from setting files
            EmailConfiguration emailConfiguration = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton<IEmailConfiguration>(emailConfiguration);
            NotificationSenderSettings notificationSenderSettings
                = Configuration.GetSection("NotificationSenderSettings").Get<NotificationSenderSettings>();
            services.AddSingleton<INotificationSenderSettings>(
                notificationSenderSettings);
            services.AddSingleton<MongoDBSettings>(
                Configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>());
            //if (!File.Exists(AppSettingsFileName))
            //{
            //    throw new Exception($"file {AppSettingsFileName} does not exist");
            //}
            //string content = File.ReadAllText(AppSettingsFileName);
            ////TODO: use Microsoft.Extensions.Configuration
            //JToken parsedJson = JObject.Parse(content)["EmailConfiguration"];
            //EmailConfiguration emailConfiguration = parsedJson.ToObject<EmailConfiguration>();
            //services.AddSingleton<IEmailConfiguration>(emailConfiguration);

            //parsedJson = JObject.Parse(content)["NotificationSenderSettings"];
            //NotificationSenderSettings notificationSenderSettings = parsedJson.ToObject<NotificationSenderSettings>();
            //services.AddSingleton<INotificationSenderSettings>(notificationSenderSettings);

            //parsedJson = JObject.Parse(content)["MongoDBSettings"];
            //MongoDBSettings mongoDBSettings = parsedJson.ToObject<MongoDBSettings>();
            //services.AddSingleton<MongoDBSettings>(mongoDBSettings);
            #endregion
            #region DatabaseConfiguration
//#if UseMySQL
//            string connection = Configuration.GetConnectionString("QueueSystem");            
//            services.AddDbContext<QueueSystemDbContext>(options => options.UseMySql(connection,
//                new MySqlServerVersion(new Version(8, 0, 27)))
//            .EnableSensitiveDataLogging(true), 
//                ServiceLifetime.Transient);
//#else
//services.AddDbContext<QueueSystemDbContext>(options => options.UseInMemoryDatabase("Notification"), 
//                ServiceLifetime.Transient);
//#endif
            if(notificationSenderSettings.QueueDatabaseType == QueueDatabaseType.MySQL)
            {
                string connection = Configuration.GetConnectionString("MySQL");
                services.AddDbContext<QueueSystemDbContext>(options => options.UseMySql(connection,
                    new MySqlServerVersion(new Version(8, 0, 27)))
                    .EnableSensitiveDataLogging(true),
                    ServiceLifetime.Transient);
            }
            else if (notificationSenderSettings.QueueDatabaseType == QueueDatabaseType.InMemory)
            {
                services.AddDbContext<QueueSystemDbContext>(options => options.UseInMemoryDatabase("Notification"),
                    ServiceLifetime.Transient);
            }
            #endregion


            //BsonDefaults.MaxSerializationDepth = 2;

            #region FluentEmail_Smtp
            SmtpClient smtp = new SmtpClient
            {
                //The address of the SMTP server (I'll take mailbox 126 as an example, which can be set according to the specific mailbox you use)
                Host = emailConfiguration.SmtpHost,
                Port = emailConfiguration.SmtpPort,
                UseDefaultCredentials = true,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //Enter the user name and password of your sending SMTP server here
                Credentials = new NetworkCredential(emailConfiguration.SmtpEmail, emailConfiguration.SmtpPassword)
            };
            services
                .AddFluentEmail(emailConfiguration.SmtpEmail)
                .AddSmtpSender(smtp); //configure host and port
            #endregion

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<INotificationService, NotificationService.Services.NotificationService>();
            services.AddTransient<INamedPipeServerService, NamedPipeServerService>();
            services.AddScoped<INotificationServiceSender, NotificationServiceSender>();
            services.AddScoped<INotificationMongoRepository, NotificationMongoRepository>();
            services.AddScoped<IProblemNotificationsService, ProblemNotificationsService>();
            servicesProvider = services.BuildServiceProvider();

            //#if UseMySQL
            if (notificationSenderSettings.QueueDatabaseType == QueueDatabaseType.MySQL)
            {
                var db = servicesProvider.GetService<QueueSystemDbContext>();
                db.Database.Migrate();
            }
            //#endif
        }
    }
}
