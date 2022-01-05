using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Repositories;
using WetPicsRebirth.Extensions;

namespace WetPicsRebirth.Commands.UserCommands.Posting;

/// <summary>
/// Sample: /wetpicson -1001411191119 15
/// </summary>
public class TurnOnPostingCommandHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IScenesRepository _scenesRepository;

    public TurnOnPostingCommandHandler(
        ITelegramBotClient telegramBotClient,
        ILogger<TurnOnPostingCommandHandler> logger,
        IMemoryCache memoryCache,
        IScenesRepository scenesRepository)
        : base(telegramBotClient, logger, memoryCache)
    {
        _telegramBotClient = telegramBotClient;
        _scenesRepository = scenesRepository;
    }

    public override IEnumerable<string> ProvidedCommands
        => "/wetpicson -1001411191119".ToEnumerable();

    protected override bool WantHandle(Message message, string? command) => command == "/wetpicson";

    protected override async Task Handle(Message message, string? command, CancellationToken cancellationToken)
    {
        var parameters = message.Text?.Split(' ') ?? Array.Empty<string>();

        if (parameters.Length != 3)
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Команда должна выглядеть как /wetpicson -1001411191119 15, " +
                "где первый параметр это айди чата или канала, " +
                "а второй количество минут, через которое происходит постинг файлов.",
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

        if (!int.TryParse(parameters[2], out var time))
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Неверное количество минут",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        if (!await CheckOnAdmin(targetId, message.From!.Id))
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "У вас должны быть права администратора в выбранном чате или канале",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        await _scenesRepository.CreateOrUpdate(targetId, time);

        await _telegramBotClient.SendTextMessageAsync(
            message.Chat.Id,
            "Постинг включен! Наслаждайтесь картиночками~",
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken);
    }
}