using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories
{
    public interface IScenesRepository
    {
        Task CreateOrUpdate(long targetChatId, int minInterval);

        Task SetPostedAt(long targetChatId, Instant now);

        Task Disable(long targetChatId);

        Task<IReadOnlyCollection<Scene>> GetEnabledAndReady();
    }
}
