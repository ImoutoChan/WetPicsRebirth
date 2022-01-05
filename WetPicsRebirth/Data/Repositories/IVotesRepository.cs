using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories;

public interface IVotesRepository
{
    Task<int> AddOrIgnore(Vote vote);
}