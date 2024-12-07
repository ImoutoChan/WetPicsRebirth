using System.Text.RegularExpressions;
using Telegram.Bot.Exceptions;
using WetPicsRebirth.Data.Repositories.Abstract;

namespace WetPicsRebirth.Services.LikesCounterUpdater;

internal partial class LikesCounterUpdater : ILikesCounterUpdater
{
    [GeneratedRegex("retry after (?<after>\\d+)")]
    private static partial Regex RetryAfterRegex();
    
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IVotesRepository _votesRepository;

    public LikesCounterUpdater(ITelegramBotClient telegramBotClient, IVotesRepository votesRepository)
    {
        _telegramBotClient = telegramBotClient;
        _votesRepository = votesRepository;
    }

    public async Task Update(MessageToUpdateCounter message) 
        => await UpdateMessageWithRetries(message.ChatId, message.MessageId, 0);

    private async Task UpdateMessageWithRetries(
        long chatId,
        int messageId,
        int retryCount,
        CancellationToken ct = default)
    {
        try
        {
            var currentCount = await _votesRepository.GetCountForPost(chatId, messageId);
            await _telegramBotClient.EditMessageReplyMarkup(
                chatId,
                messageId,
                Keyboards.WithLikes(currentCount),
                cancellationToken: ct);
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
