using System.Threading.Tasks;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories
{
    public interface IVotesRepository
    {
        Task<int> AddOrRemove(Vote vote);
    }
}
