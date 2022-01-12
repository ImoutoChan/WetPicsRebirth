namespace WetPicsRebirth.Services.UserAccounts;

public class LikesToFavoritesTranslatorHostedService : IHostedService
{
    private readonly ILikesToFavoritesTranslatorScheduler _scheduler;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LikesToFavoritesTranslatorHostedService> _logger;

    public LikesToFavoritesTranslatorHostedService(
        ILikesToFavoritesTranslatorScheduler scheduler,
        IServiceProvider serviceProvider,
        ILogger<LikesToFavoritesTranslatorHostedService> logger)
    {
        _scheduler = scheduler;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(Process, cancellationToken);

        return Task.CompletedTask;
    }

    private async Task Process()
    {
        var reader = _scheduler.VotesToTranslate.Reader;

        while (await reader.WaitToReadAsync())
        while (reader.TryRead(out var item))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var translator = scope.ServiceProvider.GetRequiredService<ILikesToFavoritesTranslator>();
                await translator.Translate(item);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Unable to fav post chat {ChatId} message {MessageId}",
                    item.ChatId,
                    item.MessageId);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _scheduler.VotesToTranslate.Writer.Complete();
        return Task.CompletedTask;
    }
}
