using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories.Abstract;

public interface IUsersRepository
{
    Task AddOrUpdate(User user);
}