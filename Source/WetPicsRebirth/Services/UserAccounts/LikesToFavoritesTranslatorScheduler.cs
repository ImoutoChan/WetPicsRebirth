using System.Threading.Channels;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Services.UserAccounts;

// singleton
internal class LikesToFavoritesTranslatorScheduler : ILikesToFavoritesTranslatorScheduler
{
    public Channel<Vote> VotesToTranslate { get; } = Channel.CreateUnbounded<Vote>();

    public async Task Schedule(Vote vote)
    {
        await VotesToTranslate.Writer.WriteAsync(vote);
    }
}
