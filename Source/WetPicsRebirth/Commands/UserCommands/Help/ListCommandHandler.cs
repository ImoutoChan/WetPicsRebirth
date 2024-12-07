using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.UserCommands.Abstract;

namespace WetPicsRebirth.Commands.UserCommands.Help;

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

    protected override async Task Handle(Message message, string? command, CancellationToken ct)
    {
        var commands = GetType().Assembly
            .GetTypes()
            .Where(x => x.IsAssignableTo(typeof(MessageHandler)))
            .Where(x => x != typeof(MessageHandler))
            .Select(x => (MessageHandler)_serviceProvider.GetRequiredService(x))
            .SelectMany(x => x.ProvidedCommands);

        await _telegramBotClient.SendMessage(
            message.Chat.Id,
            "Your hands today, sir: \n\n" + string.Join("\n", commands),
            replyParameters: message.MessageId,
            cancellationToken: ct);
    }
}
