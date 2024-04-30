using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories.Abstract;
using WetPicsRebirth.EntryPoint.Service.Notifications;
using WetPicsRebirth.Extensions;
using WetPicsRebirth.Services.LikesCounterUpdater;
using WetPicsRebirth.Services.Offload;

namespace WetPicsRebirth.Commands.UserCommands;

public class LikeCallbackHandler : ICallbackHandler
{
    private const string LikeData = "vote_l";
    private readonly ITelegramBotClient _telegramBotClient;

    private readonly IUsersRepository _usersRepository;
    private readonly IVotesRepository _votesRepository;
    private readonly IOffloader<Vote> _likesToFavorites;
    private readonly IOffloader<MessageToUpdateCounter> _likesCounterUpdater;
    private readonly ILogger<LikeCallbackHandler> _logger;

    public LikeCallbackHandler(
        IUsersRepository usersRepository,
        IVotesRepository votesRepository,
        ITelegramBotClient telegramBotClient,
        IOffloader<Vote> likesToFavorites,
        ILogger<LikeCallbackHandler> logger, 
        IOffloader<MessageToUpdateCounter> likesCounterUpdater)
    {
        _usersRepository = usersRepository;
        _votesRepository = votesRepository;
        _telegramBotClient = telegramBotClient;
        _likesToFavorites = likesToFavorites;
        _logger = logger;
        _likesCounterUpdater = likesCounterUpdater;
    }

    public async Task Handle(CallbackNotification notification, CancellationToken token)
    {
        if (notification.CallbackQuery.Data != LikeData)
            return;

        var messageId = notification.CallbackQuery.Message!.MessageId;
        var chatId = notification.CallbackQuery.Message.Chat.Id;
        var userId = notification.CallbackQuery.From.Id;

        var vote = new Vote(userId, chatId, messageId);
        var user = new User
        {
            Id = notification.CallbackQuery.From.Id,
            FirstName = notification.CallbackQuery.From.FirstName,
            LastName = notification.CallbackQuery.From.LastName,
            Username = notification.CallbackQuery.From.Username
        };

        await _usersRepository.AddOrUpdate(user);
        var counts = await _votesRepository.AddOrIgnore(vote);

        await _telegramBotClient.AnswerCallbackQueryAsync(
            notification.CallbackQuery.Id, 
            cancellationToken: token)
            .TryRun(_logger);

        if (counts <= 0)
            return;

        await _likesToFavorites.Offload(vote);
        await _likesCounterUpdater.Offload(new(chatId, messageId));
    }
}
