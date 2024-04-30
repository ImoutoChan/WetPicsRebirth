using WetPicsRebirth.Services.Offload;

namespace WetPicsRebirth.Services.LikesCounterUpdater;

public static class LikesCounterUpdaterOffloadServiceCollectionExtensions
{
    public static IServiceCollection AddLikesCounterUpdaterOffload(this IServiceCollection services)
    {
        services.AddTransient<ILikesCounterUpdater, LikesCounterUpdater>();
        services.AddOffload<MessageToUpdateCounter>(options =>
        {
            options.ItemProcessor 
                = (x, message) => x.GetRequiredService<ILikesCounterUpdater>().Update(message);

            options.ErrorLogger = (logger, message, exception) =>
                logger.LogError(
                    exception,
                    "Unable to update likes counter for chat {ChatId} message {MessageId}",
                    message.ChatId,
                    message.MessageId);
        });
        return services;
    }
}
