using Offloader;

namespace WetPicsRebirth.Services.LikesCounterUpdater;

public static class LikesCounterUpdaterOffloadServiceCollectionExtensions
{
    public static IServiceCollection AddLikesCounterUpdaterOffload(this IServiceCollection services)
        => services
            .AddTransient<ILikesCounterUpdater, LikesCounterUpdater>()
            .AddOffload<MessageToUpdateCounter>(y => y
                .UseItemProcessor((x, message, ct) => x.GetRequiredService<ILikesCounterUpdater>().Update(message))
                .UseErrorLogger((logger, message, exception) =>
                    logger.LogError(
                        exception,
                        "Unable to update likes counter for chat {ChatId} message {MessageId}",
                        message.ChatId,
                        message.MessageId)));
}
