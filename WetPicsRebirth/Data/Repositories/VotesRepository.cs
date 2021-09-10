using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories
{
    public class VotesRepository : IVotesRepository
    {
        private readonly WetPicsRebirthDbContext _context;

        public VotesRepository(WetPicsRebirthDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddOrRemove(Vote vote)
        {
            var exists = await _context.Votes.FirstOrDefaultAsync(x =>
                x.ChatId == vote.ChatId && x.MessageId == vote.MessageId && x.UserId == vote.UserId);

            if (exists != null)
            {
                _context.Votes.Remove(exists);
            }
            else
            {
                _context.Votes.Add(vote);
            }
            await _context.SaveChangesAsync();

            return await _context.Votes
                .Where(x => x.ChatId == vote.ChatId && x.MessageId == vote.MessageId)
                .CountAsync();
        }
    }
}
