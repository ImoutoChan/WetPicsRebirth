using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Models
{
    public record PixivPostHeader(int Id, string ImageUrl, string Title, string ArtistName) : PostHeader(Id, null);

    public record PixivPost(PixivPostHeader PixivPostHeader, byte[] File) : Post(PixivPostHeader, PixivPostHeader.ImageUrl, File);
}
