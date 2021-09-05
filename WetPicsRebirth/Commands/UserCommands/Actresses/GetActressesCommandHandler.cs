using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories;
using WetPicsRebirth.Extensions;

namespace WetPicsRebirth.Commands.UserCommands.Actresses
{
    /// <summary>
    /// Sample: /getactresses -1001411191119
    /// </summary>
    public class GetActressesCommandHandler : MessageHandler
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IActressesRepository _actressesRepository;

        public GetActressesCommandHandler(
            ITelegramBotClient telegramBotClient,
            ILogger<GetActressesCommandHandler> logger,
            IMemoryCache memoryCache,
            IActressesRepository actressesRepository)
            : base(telegramBotClient, logger, memoryCache)
        {
            _telegramBotClient = telegramBotClient;
            _actressesRepository = actressesRepository;
        }

        public override IEnumerable<string> ProvidedCommands
            => "/getactresses -1001411191119".ToEnumerable();

        protected override bool WantHandle(Message message, string? command) => command == "/getactresses";

        protected override async Task Handle(Message message, string? command, CancellationToken cancellationToken)
        {
            var parameters = message.Text.Split(' ');

            if (parameters.Length != 2)
            {
                await _telegramBotClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "Команда должна выглядеть как /getactress -1001411191119, " +
                    "где параметр это айди чата или канала",
                    replyToMessageId: message.MessageId,
                    cancellationToken: cancellationToken);
                return;
            }

            if (!long.TryParse(parameters[1], out var targetId))
            {
                await _telegramBotClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "Неверный айди чата или канала",
                    replyToMessageId: message.MessageId,
                    cancellationToken: cancellationToken);
                return;
            }

            if (!await CheckOnAdmin(targetId, message.From.Id))
            {
                await _telegramBotClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "У вас должны быть права администратора в выбранном чате или канале",
                    replyToMessageId: message.MessageId,
                    cancellationToken: cancellationToken);
                return;
            }

            var actresses = await _actressesRepository.GetForChat(targetId);

            if (!actresses.Any())
            {
                await _telegramBotClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "Актрисы не готовы!",
                    replyToMessageId: message.MessageId,
                    cancellationToken: cancellationToken);
                return;
            }

            var actressesListMessage = GetActressesListMessage(actresses);

            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                actressesListMessage,
                replyToMessageId: message.MessageId,
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }

        private static string GetActressesListMessage(IReadOnlyCollection<Actress> actresses)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Работают следующие актрисы");

            foreach (var actress in actresses)
            {
                sb.AppendLine();
                sb.Append("<b>");
                sb.Append(actress.ImageSource.ToString());
                sb.AppendLine("</b>");
                sb.AppendLine(actress.Options);
                sb.Append("Команда для удаления: /removeactress");
                sb.AppendLine(actress.Id.ToString().Replace("-", ""));
            }

            sb.AppendLine();

            sb.AppendLine("Для добавления используйте команду /addactress 'chatId' 'type' 'options'");

            return sb.ToString();
        }
    }
}

