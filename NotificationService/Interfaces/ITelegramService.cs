using DAL_NS.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Interfaces
{
    public interface ITelegramService
    {
        Task SendAsync(string body, long chatId);
    }
}
