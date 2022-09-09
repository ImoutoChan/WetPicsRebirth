using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.ServiceCommands.Posting;
using WetPicsRebirth.Commands.UserCommands.Abstract;

namespace WetPicsRebirth.Commands.UserCommands.Help;

public class WeeklyTopCommandHandler : MessageHandler
{
    private readonly IMediator _mediator;

    public WeeklyTopCommandHandler(
        ITelegramBotClient telegramBotClient,
        ILogger<WeeklyTopCommandHandler> logger,
        IMemoryCache memoryCache,
        IMediator mediator)
        : base(telegramBotClient, logger, memoryCache)
    {
        _mediator = mediator;
    }

    public override IEnumerable<string> ProvidedCommands => new[] { "/top" };

    protected override bool WantHandle(Message message, string? command) => command is "/top";

    protected override Task Handle(Message message, string? command, CancellationToken ct) 
        => _mediator.Send(new PostWeeklyTop(), ct);
}
