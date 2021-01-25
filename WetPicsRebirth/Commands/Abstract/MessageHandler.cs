using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WetPicsRebirth.EntryPoint.Service;

namespace WetPicsRebirth.Commands.Abstract
{
    public abstract class MessageHandler : IMessageHandler
    {
        private const string MeMemoryCacheKey = "_meMemoryCacheKey";

        private readonly IMemoryCache _memoryCache;

        protected MessageHandler(
            ITelegramBotClient telegramBotClient, 
            ILogger logger,
            IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;

            TelegramBotClient = telegramBotClient;
            Logger = logger;
        }

        protected ITelegramBotClient TelegramBotClient { get; }

        protected ILogger Logger { get; }

        public async Task Handle(MessageNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var command = await GetCommand(notification.Message, cancellationToken);

                if (!WantHandle(notification.Message, command))
                    return;

                Logger.LogInformation("Command received {Command}", command);
                await Handle(notification.Message, command, cancellationToken);
            }
            catch (Exception e)
            {
                Logger.LogError(
                    e,
                    "An error occurred while processing message {Message} {ChatId} {UserId}",
                    notification.Message, 
                    notification.Message.Chat.Id,
                    notification.Message.From.Id);
            }
        }

        private async Task<string?> GetCommand(Message message, CancellationToken cancellationToken)
        {
            var me = await GetUser(cancellationToken);
            var botUsername = me.Username;

            var text = message?.Text;

            if (string.IsNullOrWhiteSpace(text) || !text.StartsWith("/"))
                return null;

            var firstWord = text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).First();
            var isCommandWithId = firstWord.Contains("@") && firstWord.EndsWith(botUsername);
            var command = isCommandWithId ? firstWord.Split('@').First() : firstWord;

            return command;
        }

        private async Task<User> GetUser(CancellationToken cancellationToken)
        {
            return await _memoryCache.GetOrCreateAsync(
                MeMemoryCacheKey,
                entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                    return TelegramBotClient.GetMeAsync(cancellationToken);
                });
        }

        protected abstract bool WantHandle(Message message, string? command);

        protected abstract Task Handle(Message message, string? command, CancellationToken cancellationToken);
    }
}
