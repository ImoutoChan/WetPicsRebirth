using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.ServiceCommands.Posting;
using WetPicsRebirth.Commands.UserCommands.Abstract;

namespace WetPicsRebirth.Commands.UserCommands.Help;

public class MonthlyTopCommandHandler : MessageHandler
{
    private readonly IMediator _mediator;

    public MonthlyTopCommandHandler(
        ITelegramBotClient telegramBotClient,
        ILogger<MonthlyTopCommandHandler> logger,
        IMemoryCache memoryCache,
        IMediator mediator)
        : base(telegramBotClient, logger, memoryCache)
    {
        _mediator = mediator;
    }

    public override IEnumerable<string> ProvidedCommands => new[] { "/topmonth" };

    protected override bool WantHandle(Message message, string? command) => command is "/topmonth";

    protected override Task Handle(Message message, string? command, CancellationToken ct) 
        => _mediator.Send(new PostTop(TopType.Monthly), ct);
}