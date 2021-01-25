using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.Abstract;

namespace WetPicsRebirth.Commands.Help
{
    public class HelpCommandHandler : MessageHandler
    {
        public HelpCommandHandler(
            ITelegramBotClient telegramBotClient,
            ILogger<HelpCommandHandler> logger,
            IMemoryCache memoryCache) 
            : base(telegramBotClient, logger, memoryCache)
        {
        }

        protected override bool WantHandle(Message message, string? command)
        {
            return command == "/help" || command == "/start";
        }

        protected override async Task Handle(Message message, string? command, CancellationToken cancellationToken)
        {
            await TelegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Ну привет!",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
        }
    }
}
