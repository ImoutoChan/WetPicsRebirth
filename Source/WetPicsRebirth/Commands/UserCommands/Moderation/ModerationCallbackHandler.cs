using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Repositories;
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

    public async Task Handle(CallbackNotification notification, CancellationToken token)
    {
        if (notification.CallbackQuery.Data?.StartsWith(CallbackStartsWith) != true)
            return;

        var messageId = notification.CallbackQuery.Message!.MessageId;
        var chatId = notification.CallbackQuery.Message.Chat.Id;
        var isApproved = notification.CallbackQuery.Data == CallbackApproveMessage;

        await _moderatedPostsRepository.Set(messageId, isApproved);
        await _telegramBotClient.AnswerCallbackQueryAsync(notification.CallbackQuery.Id, cancellationToken: token);

        if (isApproved)
            await _telegramBotClient.DeleteMessageAsync(chatId, messageId, token);
    }
}
