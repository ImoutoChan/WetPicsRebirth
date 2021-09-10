using System.Threading.Tasks;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories
{
    public interface IUsersRepository
    {
        Task AddOrUpdate(User user);
    }
}
