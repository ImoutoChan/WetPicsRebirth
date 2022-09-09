using LinqToDB;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Data.Repositories.Abstract;

namespace WetPicsRebirth.Data.Repositories;

internal class ModeratedPostsRepository : IModeratedPostsRepository
{
    private readonly WetPicsRebirthDbContext _context;

    public ModeratedPostsRepository(WetPicsRebirthDbContext context)
    {
        _context = context;
    }

    public async Task<bool?> CheckIsApproved(int postId, string hash)
    {
        var found = await _context.ModeratedMedia.Where(x => x.PostId == postId && hash == x.Hash).FirstOrDefaultAsync();

        return found?.IsApproved;
    }

    public async Task Add(int postId, string hash, int sentMessageId)
    {
        var moderatedMedia = new ModeratedMedia(Guid.NewGuid(), postId, hash, sentMessageId, false);

        _context.ModeratedMedia.Add(moderatedMedia);
        await _context.SaveChangesAsync();
    }

    public async Task Set(int messageId, bool isApproved)
    {
        var found = await _context.ModeratedMedia.Where(x => x.MessageId == messageId).FirstOrDefaultAsync();

        if (found == null)
            throw new InvalidOperationException($"Unable to find ModerationMedia with messageId == {messageId}");

        found.IsApproved = isApproved;
        await _context.SaveChangesAsync();
    }
}