using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories
{
    public class PostedMediaRepository : IPostedMediaRepository
    {
        private readonly WetPicsRebirthDbContext _context;

        public PostedMediaRepository(WetPicsRebirthDbContext context)
        {
            _context = context;
        }

        public async Task Add(long chatId, int messageId, string fileId, ImageSource imageSource, int postId)
        {
            var posted = new PostedMedia(Guid.NewGuid(), chatId, messageId, fileId, imageSource, postId);
            _context.PostedMedia.Add(posted);
            await _context.SaveChangesAsync();
        }

        public async Task<int?> GetFirstNew(long chatId, ImageSource imageSource, IReadOnlyCollection<int> postIds)
        {
            var alreadyPosted = await _context.PostedMedia
                .Where(x => x.ChatId == chatId && x.ImageSource == imageSource)
                .Select(x => x.PostId)
                .Where(x => postIds.Contains(x))
                .ToListAsync();

            var result = postIds.Except(alreadyPosted).FirstOrDefault();

            return result != 0 ? result : null;
        }
    }
}
