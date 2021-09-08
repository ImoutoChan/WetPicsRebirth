using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories
{
    public interface IScenesRepository
    {
        Task CreateOrUpdate(long targetChatId, int minInterval);

        Task SetPostedAt(long targetChatId, DateTimeOffset now);

        Task Disable(long targetChatId);

        Task<IReadOnlyCollection<Scene>> GetEnabledAndReady();
    }
}
