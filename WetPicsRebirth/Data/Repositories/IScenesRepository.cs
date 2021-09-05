using System.Threading.Tasks;

namespace WetPicsRebirth.Data.Repositories
{
    public interface IScenesRepository
    {
        Task CreateOrUpdate(long targetChatId, int minInterval);

        Task Disable(long targetChatId);
    }
}
