using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories;
using WetPicsRebirth.Extensions;

namespace WetPicsRebirth.Commands.UserCommands.Actresses;

/// <summary>
/// Sample: /addactress -1001411191119
/// pixiv/danbooru/yandere
/// d[day|week|month]
/// y[day|week|month]
/// p[dailygeneral|dailyr18|weeklygeneral|weeklyr18|monthly|rookie|
/// original|bymalegeneral|bymaler18|byfemalegeneral|byfemaler18|r18g]
/// </summary>
public class AddActressCommandHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IActressesRepository _actressesRepository;

    public AddActressCommandHandler(
        ITelegramBotClient telegramBotClient,
        ILogger<AddActressCommandHandler> logger,
        IMemoryCache memoryCache,
        IActressesRepository actressesRepository)
        : base(telegramBotClient, logger, memoryCache)
    {
        _telegramBotClient = telegramBotClient;
        _actressesRepository = actressesRepository;
    }

    public override IEnumerable<string> ProvidedCommands
        => "/addactress -1001411191119 yandere month".ToEnumerable();

    protected override bool WantHandle(Message message, string? command) => command == "/addactress";

    protected override async Task Handle(Message message, string? command, CancellationToken cancellationToken)
    {
        var parameters = message.Text?.Split(' ') ?? Array.Empty<string>();

        if (parameters.Length != 4)
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Команда должна выглядеть как /addactress -1001411191119 yandere month, " +
                "где первый параметр это айди чата или канала, " +
                "второй — источник картинок, " +
                "третий — параметр источника.",
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

        if (!await CheckOnAdmin(targetId, message.From!.Id))
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "У вас должны быть права администратора в выбранном чате или канале",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        if (!TryGetImageSource(parameters[2], out var source))
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Неверный источник, доступны: pixiv, yandere, danbooru",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        if (!TryGetImageSourceOptions(parameters[3], source, out var options))
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Неверная опция для источника, доступны: day|week|month " +
                "или dailygeneral|dailyr18|weeklygeneral|weeklyr18|monthly|" +
                "rookie|original|bymalegeneral|bymaler18|byfemalegeneral|byfemaler18|r18g",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        await _actressesRepository.Add(targetId, source, options);

        await _telegramBotClient.SendTextMessageAsync(
            message.Chat.Id,
            "Актрисы прихорошились и готовы к выступлению!",
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken);
    }

    private static bool TryGetImageSourceOptions(
        string optionsString,
        ImageSource source,
        [NotNullWhen(true)] out string? options)
    {
        var availableForBooru = new [] {"day", "month", "week"};
        var availableForPixiv = new[]
        {
            "dailygeneral", "dailyr18", "weeklygeneral", "weeklyr18", "monthly", "rookie", "original",
            "bymalegeneral", "bymaler18", "byfemalegeneral", "byfemaler18", "r18g"
        };

        if (source is ImageSource.Danbooru or ImageSource.Yandere)
        {
            var lowerOptions = optionsString.ToLowerInvariant();
            if (availableForBooru.Contains(lowerOptions))
            {
                options = lowerOptions;
                return true;
            }
        }

        if (source is ImageSource.Pixiv)
        {
            var lowerOptions = optionsString.ToLowerInvariant();
            if (availableForPixiv.Contains(lowerOptions))
            {
                options = lowerOptions;
                return true;
            }
        }

        options = null;
        return false;
    }

    private static bool TryGetImageSource(string sourceString, out ImageSource source)
    {
        source = default;

        if (!Enum.TryParse(typeof(ImageSource), sourceString, true, out var result))
            return false;

        source = (ImageSource)result!;
        return true;
    }
}