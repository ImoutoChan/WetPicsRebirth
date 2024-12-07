using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;
using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories.Abstract;
using WetPicsRebirth.Extensions;

namespace WetPicsRebirth.Commands.UserCommands.Actresses;

/// <summary>
/// Sample: /getactresses -1001411191119
/// </summary>
public class GetActressesCommandHandler : MessageHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IActressesRepository _actressesRepository;

    public GetActressesCommandHandler(
        ITelegramBotClient telegramBotClient,
        ILogger<GetActressesCommandHandler> logger,
        IMemoryCache memoryCache,
        IActressesRepository actressesRepository)
        : base(telegramBotClient, logger, memoryCache)
    {
        _telegramBotClient = telegramBotClient;
        _actressesRepository = actressesRepository;
    }

    public override IEnumerable<string> ProvidedCommands
        => "/getactresses -1001411191119".ToEnumerable();

    protected override bool WantHandle(Message message, string? command) => command == "/getactresses";

    protected override async Task Handle(Message message, string? command, CancellationToken ct)
    {
        var parameters = message.Text?.Split(' ') ?? Array.Empty<string>();

        if (parameters.Length != 2)
        {
            await _telegramBotClient.SendMessage(
                message.Chat.Id,
                "Команда должна выглядеть как /getactress -1001411191119, " +
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

        var actresses = await _actressesRepository.GetForChat(targetId);

        if (!actresses.Any())
        {
            await _telegramBotClient.SendMessage(
                message.Chat.Id,
                "Актрисы не готовы!",
                replyParameters: message.MessageId,
                cancellationToken: ct);
            return;
        }

        var actressesListMessage = GetActressesListMessage(actresses);

        await _telegramBotClient.SendMessage(
            message.Chat.Id,
            actressesListMessage,
            replyParameters: message.MessageId,
            parseMode: ParseMode.Html,
            cancellationToken: ct);
    }

    private static string GetActressesListMessage(IReadOnlyCollection<Actress> actresses)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Работают следующие актрисы");

        foreach (var actress in actresses)
        {
            sb.AppendLine();
            sb.Append("<b>");
            sb.Append(actress.ImageSource.ToString());
            sb.Append("</b> ");
            sb.AppendLine(actress.Options);
            sb.Append("/removeactress_");
            sb.AppendLine(actress.Id.ToString().Replace("-", ""));
        }

        sb.AppendLine();

        sb.AppendLine("Для добавления используйте команду /addactress 'chatId' 'type' 'options'");

        return sb.ToString();
    }
}
