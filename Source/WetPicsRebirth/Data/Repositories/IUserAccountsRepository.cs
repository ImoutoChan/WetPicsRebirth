using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories;

public interface IUserAccountsRepository
{
    Task<UserAccount> Set(long userId, ImageSource source, string login, string apikey);

    Task<UserAccount?> Get(long userId, ImageSource source);
}
