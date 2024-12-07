using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.UserCommands.Abstract;

namespace WetPicsRebirth.Commands.UserCommands.Help;

public class HelpCommandHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;

    public HelpCommandHandler(
        ITelegramBotClient telegramBotClient,
        ILogger<HelpCommandHandler> logger,
        IMemoryCache memoryCache)
        : base(telegramBotClient, logger, memoryCache)
    {
        _telegramBotClient = telegramBotClient;
    }

    public override IEnumerable<string> ProvidedCommands
        => ["/help", "/start"];

    protected override bool WantHandle(Message message, string? command)
        => command is "/help" or "/start";

    protected override async Task Handle(Message message, string? command, CancellationToken ct)
    {
        await _telegramBotClient.SendMessage(
            message.Chat.Id,
            "Girls are getting ready, please kindly wait",
            replyParameters: message.MessageId,
            cancellationToken: ct);
    }
}
