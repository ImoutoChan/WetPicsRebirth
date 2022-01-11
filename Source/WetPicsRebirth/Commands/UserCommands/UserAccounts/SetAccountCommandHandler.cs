using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories;
using WetPicsRebirth.Extensions;

namespace WetPicsRebirth.Commands.UserCommands.UserAccounts;

/// <summary>
/// Sample: /setaccount danbooru username sad1234jf3kl312aj341jkl123
/// danbooru/yandere
/// login apikey
/// </summary>
public class SetAccountCommandHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IUserAccountsRepository _userAccountsRepository;

    public SetAccountCommandHandler(
        ITelegramBotClient telegramBotClient,
        ILogger<SetAccountCommandHandler> logger,
        IMemoryCache memoryCache,
        IUserAccountsRepository userAccountsRepository)
        : base(telegramBotClient, logger, memoryCache)
    {
        _telegramBotClient = telegramBotClient;
        _userAccountsRepository = userAccountsRepository;
    }

    public override IEnumerable<string> ProvidedCommands
        => "/setaccount danbooru username sad1234jf3kl312aj341jkl123".ToEnumerable();

    protected override bool WantHandle(Message message, string? command) => command == "/setaccount";

    protected override async Task Handle(Message message, string? command, CancellationToken cancellationToken)
    {
        var userId = message.From!.Id;

        var parameters = message.Text?.Split(' ') ?? Array.Empty<string>();

        if (parameters.Length != 4)
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Команда должна выглядеть как /setaccount danbooru username sad1234jf3kl312aj341jkl123, " +
                "где первый параметр это источник картинок, " +
                "второй — ваш логин в источнике, " +
                "третий — ваш ключ ApiKey (обычно можно посмотреть на сайте в профиле).",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        if (!TryGetImageSource(parameters[1], out var source) || source == ImageSource.Pixiv)
        {
            await _telegramBotClient.SendTextMessageAsync(
                message.Chat.Id,
                "Неверный источник, доступны: yandere, danbooru",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        var login = parameters[2];
        var apikey = parameters[3];

        await _userAccountsRepository.Set(userId, source, login, apikey);

        await _telegramBotClient.SendTextMessageAsync(
            message.Chat.Id,
            "Ваш персональный пропуск к актрисам сохранен!",
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken);
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
