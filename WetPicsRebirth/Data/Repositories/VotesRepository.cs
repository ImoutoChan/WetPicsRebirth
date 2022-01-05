using Microsoft.EntityFrameworkCore;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories;

public class VotesRepository : IVotesRepository
{
    private readonly WetPicsRebirthDbContext _context;

    public VotesRepository(WetPicsRebirthDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddOrIgnore(Vote vote)
    {
        var now = SystemClock.Instance.GetCurrentInstant();

        var affected = await _context.Database.ExecuteSqlInterpolatedAsync(
            $@"INSERT INTO ""Votes"" (""UserId"", ""ChatId"", ""MessageId"", ""AddedDate"", ""ModifiedDate"")
                   VALUES ({vote.UserId}, {vote.ChatId}, {vote.MessageId}, {now}, {now})
                   ON CONFLICT DO NOTHING");

        if (affected > 0)
            return await _context.Votes.CountAsync(x => x.ChatId == vote.ChatId && x.MessageId == vote.MessageId);

        return -1;
    }
}
