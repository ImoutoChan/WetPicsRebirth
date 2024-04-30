using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Services.Offload;

namespace WetPicsRebirth.Services.UserAccounts;

public static class LikesToFavoritesOffloadServiceCollectionExtensions
{
    public static IServiceCollection AddLikesToFavoritesOffload(this IServiceCollection services)
    {
        services.AddTransient<ILikesToFavoritesTranslator, LikesToFavoritesTranslator>();
        services.AddOffload<Vote>(options =>
        {
            options.ItemProcessor = (x, vote) => x.GetRequiredService<ILikesToFavoritesTranslator>().Translate(vote);

            options.ErrorLogger = (logger, vote, exception) =>
                logger.LogError(
                    exception,
                    "Unable to fav post chat {ChatId} message {MessageId}",
                    vote.ChatId,
                    vote.MessageId);
        });
        return services;
    }
}
