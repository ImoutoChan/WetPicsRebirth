using System.Threading.Channels;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Services.UserAccounts;

public interface ILikesToFavoritesTranslatorScheduler
{
    Task Schedule(Vote vote);
    
    Channel<Vote> VotesToTranslate { get; }
}