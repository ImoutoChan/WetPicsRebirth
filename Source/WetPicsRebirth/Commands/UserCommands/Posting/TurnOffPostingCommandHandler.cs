﻿using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Repositories.Abstract;
using WetPicsRebirth.Extensions;

namespace WetPicsRebirth.Commands.UserCommands.Posting;

/// <summary>
/// Sample: /wetpicsoff -1001411191119
/// </summary>
public class TurnOffPostingCommandHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IScenesRepository _scenesRepository;

    public TurnOffPostingCommandHandler(
        ITelegramBotClient telegramBotClient,
        ILogger<TurnOffPostingCommandHandler> logger,
        IMemoryCache memoryCache,
        IScenesRepository scenesRepository)
        : base(telegramBotClient, logger, memoryCache)
    {
        _telegramBotClient = telegramBotClient;
        _scenesRepository = scenesRepository;
    }

    public override IEnumerable<string> ProvidedCommands
        => "/wetpicsoff -1001411191119".ToEnumerable();

    protected override bool WantHandle(Message message, string? command) => command == "/wetpicsoff";

    protected override async Task Handle(Message message, string? command, CancellationToken ct)
    {
        var parameters = message.Text?.Split(' ') ?? Array.Empty<string>();

        if (parameters.Length != 2)
        {
            await _telegramBotClient.SendMessage(
                message.Chat.Id,
                "Команда должна выглядеть как /wetpicsoff -1001411191119, " +
                "где параметр это айди чата или канала",
                replyParameters: message.MessageId,
                cancellationToken: ct);
            return;
        }

        if (!long.TryParse(parameters[1], out var targetId))
        {
            await _telegramBotClient.SendMessage(
                message.Chat.Id,
                "Неверный айди чата или канала",
                replyParameters: message.MessageId,
                cancellationToken: ct);
            return;
        }

        if (!await CheckOnAdmin(targetId, message.From!.Id))
        {
            await _telegramBotClient.SendMessage(
                message.Chat.Id,
                "У вас должны быть права администратора в выбранном чате или канале",
                replyParameters: message.MessageId,
                cancellationToken: ct);
            return;
        }

        await _scenesRepository.Disable(targetId);

        await _telegramBotClient.SendMessage(
            message.Chat.Id,
            "Постинг выключен! Плак..",
            replyParameters: message.MessageId,
            cancellationToken: ct);
    }
}
