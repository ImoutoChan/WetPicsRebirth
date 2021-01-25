using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace WetPicsRebirth.EntryPoint.Service
{
    public class NotificationService : INotificationService, INotification
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IMediator _mediator;

        public NotificationService(
            ILogger<NotificationService> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task NotifyAsync(Update update)
        {
            _logger.LogInformation($"Notification about {update.Type}");

            try
            {
                var notification = GetNotification(update);

                if (notification == null)
                    return;

                await _mediator.Publish(notification);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to process the request");
            }
        }

        private static INotification? GetNotification(Update update) 
            => update.Type switch
            {
                UpdateType.CallbackQuery => new CallbackNotification(update.CallbackQuery),
                UpdateType.Message => new MessageNotification(update.Message),
                _ => null
            };
    }
}