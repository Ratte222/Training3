using BLL.Interfaces;
using BLL.Services;
using DAL.EF;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BLL.Events;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using DAL.Entity;
using System.Net.Mail;
using System.Net;
using BLL.Interfaces.NamedPipe;
using BLL.Services.NamedPipe;
using BLL.Helpers;

namespace Training3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
               .AddJsonFile("InitData.json")
               .AddJsonFile("InitNotificationData.json")
               .AddConfiguration(configuration);
            Configuration = builder.Build();
            //Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            services.Configure<BrotliCompressionProvider>(options =>
            {
                
            });
            services.AddResponseCompression(options =>
            {
                //options.Providers.Add<BrotliCompressionProvider>();
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                //options.Providers.Add<BrotliCompressionProvider>();
            });
            services.AddControllers();

            #region DatabaseConfig
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDBContext>(options => options.UseMySql(connection/*, new MySqlServerVersion(new Version(8, 0, 27))*/),
                ServiceLifetime.Transient);

            services.AddDbContext<QueueSystemDbContext>(options => options.UseInMemoryDatabase("Notification"));
            #endregion

            #region Config
            var mailAddresConfigSection = Configuration.GetSection("EmailConfiguration");
            //services.Configure<SmtpConfig>(mailAddresConfigSection);
            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration")
                .Get<EmailConfiguration>());
            services.AddSingleton<MongoDBSettings>(Configuration.GetSection("MongoDBSettings")
                .Get<MongoDBSettings>());
            var smtpConfig = mailAddresConfigSection.Get<EmailConfiguration>();
            #endregion

            //#region FluentEmail_Smtp
            //SmtpClient smtp = new SmtpClient
            //{
            //    //The address of the SMTP server (I'll take mailbox 126 as an example, which can be set according to the specific mailbox you use)
            //    Host = smtpConfig.SmtpHost,
            //    Port = smtpConfig.SmtpPort,
            //    UseDefaultCredentials = true,
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    //Enter the user name and password of your sending SMTP server here
            //    Credentials = new NetworkCredential(smtpConfig.SmtpEmail, smtpConfig.SmtpPassword)
            //};
            //services
            //    .AddFluentEmail(smtpConfig.SmtpEmail)
            //    .AddSmtpSender(smtp); //configure host and port
            //#endregion

            #region Mapster
            var config = new TypeAdapterConfig();
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            #endregion

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Training3 API", Version = "v1", });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Type = SecuritySchemeType.Http,
                //    BearerFormat = "JWT",
                //    In = ParameterLocation.Header,
                //    Scheme = "bearer",
                //    Description = "Please insert JWT token into field"
                //});

                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            }
                //        },
                //        new string[] { }
                //    }
                //});
            });
            #endregion

            services.AddSingleton<ExpenseEvents>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<INotificationMongoRepository, NotificationMongoRepository>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddTransient<INamedPipeClientService, NamedPipeClientService>();

            //services.AddHostedService<NotificationServiceBackground>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDBContext applicationContext,
            QueueSystemDbContext queueSystemDbContext)
        {
            applicationContext.Database.Migrate();
            DbInitializer.Initialize(applicationContext, 
                Configuration.GetSection("Categories").Get<List<Category>>(),
                Configuration.GetSection("Expenses").Get<List<Expense>>());
            //var notifications = Configuration.GetSection("Notifications").Get<List<Notification>>();
            //DbInitializer.Initialize(queueSystemDbContext, notifications);
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseResponseCompression();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
