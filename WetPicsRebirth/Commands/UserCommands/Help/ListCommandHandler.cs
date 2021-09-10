using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.UserCommands.Abstract;

namespace WetPicsRebirth.Commands.UserCommands.Help
{
    public class ListCommandHandler : MessageHandler
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IServiceProvider _serviceProvider;

        public ListCommandHandler(
            ITelegramBotClient telegramBotClient,
            ILogger<ListCommandHandler> logger,
            IMemoryCache memoryCache,
            IServiceProvider serviceProvider)
            : base(telegramBotClient, logger, memoryCache)
        {
            _telegramBotClient = telegramBotClient;
            _serviceProvider = serviceProvider;
        }

        public override IEnumerable<string> ProvidedCommands
            => new[] { "/list" };

        protected override bool WantHandle(Message message, string? command)
            => command is "/list";

        protected override async Task Handle(Message message, string? command, CancellationToken cancellationToken)
        {
            var commands = GetType().Assembly
                .GetTypes()
                .Where(x => x.IsAssignableTo(typeof(MessageHandler)))
                .Where(x => x != typeof(MessageHandler))
                .Select(x => (MessageHandler)_serviceProvider.GetRequiredService(x))
                .SelectMany(x => x.ProvidedCommands);

            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Your hands today, sir: \n\n" + string.Join("\n", commands),
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
        }
    }
}
