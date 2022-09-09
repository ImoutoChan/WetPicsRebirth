using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories.Abstract;

public interface IScenesRepository
{
    Task CreateOrUpdate(long targetChatId, int minInterval);

    Task SetPostedAt(long targetChatId, Instant now);

    Task Disable(long targetChatId);

    Task<IReadOnlyCollection<Scene>> GetEnabledAndReady();

    Task<IReadOnlyCollection<Scene>> GetAll();
}
