using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Services.UserAccounts;

public interface ILikesToFavoritesTranslator
{
    Task Translate(Vote vote);
}