using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Repositories.Abstract;
using WetPicsRebirth.Extensions;

namespace WetPicsRebirth.Commands.UserCommands.Actresses;

/// <summary>
/// Sample: /removeactress_6B29FC40CA471067B31D00DD010662DA
/// </summary>
public class RemoveActressCommandHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IActressesRepository _actressesRepository;

    public RemoveActressCommandHandler(
        ITelegramBotClient telegramBotClient,
        ILogger<RemoveActressCommandHandler> logger,
        IMemoryCache memoryCache,
        IActressesRepository actressesRepository)
        : base(telegramBotClient, logger, memoryCache)
    {
        _telegramBotClient = telegramBotClient;
        _actressesRepository = actressesRepository;
    }

    public override IEnumerable<string> ProvidedCommands
        => "/removeactress_6B29FC40CA471067B31D00DD010662DA".ToEnumerable();

    protected override bool WantHandle(Message message, string? command)
        => command?.StartsWith("/removeactress") ?? false;

    protected override async Task Handle(Message message, string? command, CancellationToken cancellationToken)
    {
        var parameters = message.Text?.Split(' ') ?? Array.Empty<string>();

        if (parameters.Length != 1)
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Команда должна выглядеть как /removeactress_6B29FC40CA471067B31D00DD010662DA, " +
                "где первый параметр это айди актрисы",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        if (parameters[0].Length < 33 || !Guid.TryParse(parameters[0][^32..], out var actressId))
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Неверный айди актрисы",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        await _actressesRepository.Remove(actressId);

        await _telegramBotClient.SendTextMessageAsync(
            message.Chat.Id,
            "Вы уволили актрису, что поделать..",
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken);
    }
}