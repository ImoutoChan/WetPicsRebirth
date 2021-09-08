using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories
{
    internal class ScenesRepository : IScenesRepository
    {
        private readonly WetPicsRebirthDbContext _context;

        public ScenesRepository(WetPicsRebirthDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Scene>> GetEnabledAndReady()
        {
            var now = DateTimeOffset.Now;

            var enabled = await _context.Scenes
                .Where(x => x.Enabled)
                .ToListAsync();

            return enabled
                .Where(x => x.LastPostedTime != null)
                .Where(x => x.LastPostedTime!.Value.AddMinutes(x.MinutesInterval) <= now)
                .ToList();
        }

        public async Task CreateOrUpdate(long targetChatId, int minInterval)
        {
            var existing = await _context.Scenes
                .Where(x => x.ChatId == targetChatId)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                await _context.Scenes.AddAsync(new Scene(targetChatId, minInterval, DateTimeOffset.MinValue, true));
            }
            else
            {
                existing.MinutesInterval = minInterval;
                existing.Enabled = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SetPostedAt(long targetChatId, DateTimeOffset now)
        {
            var existing = await _context.Scenes
                .Where(x => x.ChatId == targetChatId)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                existing.LastPostedTime = now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task Disable(long targetChatId)
        {
            var existing = await _context.Scenes
                .Where(x => x.ChatId == targetChatId)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                existing.Enabled = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
