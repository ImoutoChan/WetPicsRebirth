using System.Text.RegularExpressions;
using Telegram.Bot.Exceptions;
using WetPicsRebirth.Commands.UserCommands.Abstract;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories.Abstract;
using WetPicsRebirth.EntryPoint.Service.Notifications;
using WetPicsRebirth.Extensions;
using WetPicsRebirth.Services;
using WetPicsRebirth.Services.UserAccounts;

namespace WetPicsRebirth.Commands.UserCommands;

public partial class LikeCallbackHandler : ICallbackHandler
{
    [GeneratedRegex("retry after (?<after>\\d+)")]
    private static partial Regex RetryAfterRegex();
    
    private const string LikeData = "vote_l";
    private readonly ITelegramBotClient _telegramBotClient;

    private readonly IUsersRepository _usersRepository;
    private readonly IVotesRepository _votesRepository;
    private readonly ILikesToFavoritesTranslatorScheduler _likesToFavoritesTranslatorScheduler;
    private readonly ILogger<LikeCallbackHandler> _logger;

    public LikeCallbackHandler(
        IUsersRepository usersRepository,
        IVotesRepository votesRepository,
        ITelegramBotClient telegramBotClient,
        ILikesToFavoritesTranslatorScheduler likesToFavoritesTranslatorScheduler,
        ILogger<LikeCallbackHandler> logger)
    {
        _usersRepository = usersRepository;
        _votesRepository = votesRepository;
        _telegramBotClient = telegramBotClient;
        _likesToFavoritesTranslatorScheduler = likesToFavoritesTranslatorScheduler;
        _logger = logger;
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

        await _likesToFavoritesTranslatorScheduler.Schedule(vote);
        await UpdateMessageWithRetries(chatId, messageId, 0, token);
    }

    private async Task UpdateMessageWithRetries(
        long chatId,
        int messageId,
        int retryCount,
        CancellationToken ct)
    {
        try
        {
            var currentCount = await _votesRepository.GetCountForPost(chatId, messageId);
            await _telegramBotClient.EditMessageReplyMarkupAsync(
                chatId,
                messageId,
                Keyboards.WithLikes(currentCount),
                ct);
        }
        catch (ApiRequestException e) when (e.Message.Contains("retry after"))
        {
            if (retryCount > 2)
                throw;

            var after = int.Parse(RetryAfterRegex().Match(e.Message).Groups["after"].Value);
            await Task.Delay(after * 1000, ct);
            await UpdateMessageWithRetries(chatId, messageId, retryCount + 1, ct);
        }
    }
}
