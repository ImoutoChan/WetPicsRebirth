using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Repositories.Abstract;
using WetPicsRebirth.EntryPoint.Service.Notifications;

namespace WetPicsRebirth.Commands.UserCommands.Moderation;

public class ModerationCallbackHandler : ICallbackHandler
{
    private const string CallbackStartsWith = "moderate_";
    private const string CallbackApproveMessage = CallbackStartsWith + "approve";

    private readonly ITelegramBotClient _telegramBotClient;

    private readonly IModeratedPostsRepository _moderatedPostsRepository;

    public ModerationCallbackHandler(
        ITelegramBotClient telegramBotClient,
        IModeratedPostsRepository moderatedPostsRepository)
    {
        _telegramBotClient = telegramBotClient;
        _moderatedPostsRepository = moderatedPostsRepository;
    }

    public async Task Handle(CallbackNotification notification, CancellationToken ct)
    {
        if (notification.CallbackQuery.Data?.StartsWith(CallbackStartsWith) != true)
            return;

        var messageId = notification.CallbackQuery.Message!.MessageId;
        var chatId = notification.CallbackQuery.Message.Chat.Id;
        var isApproved = notification.CallbackQuery.Data == CallbackApproveMessage;

        await _moderatedPostsRepository.Set(messageId, isApproved);
        await _telegramBotClient.AnswerCallbackQuery(notification.CallbackQuery.Id, cancellationToken: ct);

        if (isApproved)
        {
            await _telegramBotClient.DeleteMessage(chatId, messageId, ct);
        }
        else
        {
            await _telegramBotClient.EditMessageReplyMarkup(chatId, messageId, null, cancellationToken: ct);
        }
    }
}
