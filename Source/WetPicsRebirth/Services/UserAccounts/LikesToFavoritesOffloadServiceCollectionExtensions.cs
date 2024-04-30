using Offloader;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Services.UserAccounts;

public static class LikesToFavoritesOffloadServiceCollectionExtensions
{
    public static IServiceCollection AddLikesToFavoritesOffload(this IServiceCollection services)
        => services
            .AddTransient<ILikesToFavoritesTranslator, LikesToFavoritesTranslator>()
            .AddOffload<Vote>(y => y
                .UseItemProcessor((x, vote, ct) => x.GetRequiredService<ILikesToFavoritesTranslator>().Translate(vote))
                .UseErrorLogger((logger, vote, exception) =>
                    logger.LogError(
                        exception,
                        "Unable to fav post chat {ChatId} message {MessageId}",
                        vote.ChatId,
                        vote.MessageId)));
}
