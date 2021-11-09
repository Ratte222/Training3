using AuxiliaryLib.BaseBackgroundService;
using BLL.Interfaces;
using DAL.EF;
using DAL.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class NotificationServiceBackground : BackgroundService
    {
        private QueueSystemDbContext _context;
        private IEmailService _emailService;
        private readonly IServiceProvider _services;
        public NotificationServiceBackground(IServiceProvider Services)
        {
            _services = Services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            var scope = _services.CreateScope();
            _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            _context = scope.ServiceProvider.GetRequiredService<QueueSystemDbContext>();
            //var builder = new ConfigurationBuilder().AddJsonFile("InitNotificationData.json");
            //var configuration = 
            //var _notifications = configuration.GetSection("Notifications").Get<List<Notification>>();
            //DbInitializer.Initialize(_context, _notifications);
            while (!stoppingToken.IsCancellationRequested)
            {
                var notifications = _context.Notifications.ToList();
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
