using Microsoft.EntityFrameworkCore;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories;

public class PostedMediaRepository : IPostedMediaRepository
{
    private readonly WetPicsRebirthDbContext _context;

    public PostedMediaRepository(WetPicsRebirthDbContext context)
    {
        _context = context;
    }

    public async Task<int?> GetFirstNew(
        long chatId,
        ImageSource imageSource,
        List<(int Id, string? Md5Hash)> postIdsWithHashes)
    {
        var ids = postIdsWithHashes.Select(x => x.Id).ToList();
        var hashes = postIdsWithHashes.Where(x => x.Md5Hash != null).Select(x => x.Md5Hash).ToList();

        var alreadyPostedIds = await _context.PostedMedia
            .Where(x => x.ChatId == chatId && x.ImageSource == imageSource)
            .Select(x => x.PostId)
            .Where(x => ids.Contains(x))
            .ToListAsync();

        var alreadyPostedHashes = await _context.PostedMedia
            .Where(x => x.ChatId == chatId && x.ImageSource == imageSource)
            .Select(x => x.PostHash)
            .Where(x => hashes.Contains(x))
            .ToListAsync();

        var result = postIdsWithHashes
            .Where(x => alreadyPostedIds.All(y => y != x.Id)
                        && (x.Md5Hash == null || alreadyPostedHashes.All(y => y != x.Md5Hash)))
            .Select(x => x.Id)
            .FirstOrDefault();

        return result != 0 ? result : null;
    }

    public Task<PostedMedia?> Get(long chatId, int messageId)
        => _context.PostedMedia.FirstOrDefaultAsync(x => x.ChatId == chatId && x.MessageId == messageId);

    public async Task Add(
        long chatId,
        int messageId,
        string fileId,
        ImageSource imageSource,
        int postId,
        string postHash)
    {
        var posted = new PostedMedia(Guid.NewGuid(), chatId, messageId, fileId, imageSource, postId, postHash);
        _context.PostedMedia.Add(posted);
        await _context.SaveChangesAsync();
    }
}
