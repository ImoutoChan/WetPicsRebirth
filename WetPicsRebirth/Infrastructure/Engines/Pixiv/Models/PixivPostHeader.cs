using System.IO;
using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Models
{
    public record PixivPostHeader(int Id, string ImageUrl, string Title, string ArtistName) : PostHeader(Id, null);

    public record PixivPost(PixivPostHeader PixivPostHeader, Stream File, long FileLength)
        : Post(PixivPostHeader, PixivPostHeader.ImageUrl, File, FileLength);
}
