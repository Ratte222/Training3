using Microsoft.Extensions.Logging;
using NotificationService.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace NotificationService.Services
{
    public class TelegramService:ITelegramService
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly ILogger<TelegramService> _logger;
        public TelegramService(TelegramBotClient telegramBotClient, ILogger<TelegramService> logger)
        {
            _telegramBotClient = telegramBotClient;
            _logger = logger;
        }

        public async Task SendAsync(string body, long chatId)
        {
            var result = await _telegramBotClient.SendTextMessageAsync(chatId, body);
            _logger.LogInformation("messageId = {0}, was sent to chatId {1}", result.MessageId, chatId);
        }
    }
}
