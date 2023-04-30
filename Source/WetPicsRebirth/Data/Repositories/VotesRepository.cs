using Microsoft.EntityFrameworkCore;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories.Abstract;

namespace WetPicsRebirth.Data.Repositories;

public class VotesRepository : IVotesRepository
{
    private readonly WetPicsRebirthDbContext _context;

    public VotesRepository(WetPicsRebirthDbContext context) => _context = context;

    public async Task<int> AddOrIgnore(Vote vote)
    {
        var now = SystemClock.Instance.GetCurrentInstant();

        var affected = await _context.Database.ExecuteSqlInterpolatedAsync(
            $"""
            INSERT INTO "Votes" ("UserId", "ChatId", "MessageId", "AddedDate", "ModifiedDate")
            VALUES ({vote.UserId}, {vote.ChatId}, {vote.MessageId}, {now}, {now})
            ON CONFLICT DO NOTHING
            """);

        if (affected > 0)
            return await _context.Votes.CountAsync(x => x.ChatId == vote.ChatId && x.MessageId == vote.MessageId);

        return -1;
    }

    public async Task<IReadOnlyCollection<PostedMedia>> GetTop(int count, int forLastDays)
    {
        var now = SystemClock.Instance.GetCurrentInstant();
        
        var ids = await _context.Votes
            .Join(
                _context.PostedMedia, 
                x => new { x.ChatId, x.MessageId }, 
                x => new { x.ChatId, x.MessageId }, 
                (x, y) =>new
                {
                    Vote = x,
                    Post = y
                })
            .Where(x => now - x.Post.AddedDate < Duration.FromDays(forLastDays))
            .GroupBy(x => x.Post.Id)
            .OrderByDescending(x => x.Count())
            .Take(count)
            .Select(x => x.Key)
            .ToListAsync();

        var posts = await _context.PostedMedia.Where(x => ids.Contains(x.Id)).ToListAsync();

        return ids.Select(x => posts.First(y => y.Id == x)).ToList();
    }

    public Task<int> GetCountForPost(long chatId, int messageId) 
        => _context.Votes.CountAsync(x => x.ChatId == chatId && x.MessageId == messageId);
}
