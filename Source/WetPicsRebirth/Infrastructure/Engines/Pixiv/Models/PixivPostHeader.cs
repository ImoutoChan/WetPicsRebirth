using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv.Models;

public record PixivPostHeader(
    int Id,
    string ImageUrl,
    string Title,
    string ArtistName,
    IReadOnlyCollection<string> Tags) : PostHeader(Id, null);

public record PixivPost(PixivPostHeader PixivPostHeader, Stream File, long FileLength)
    : Post(PixivPostHeader, PixivPostHeader.ImageUrl, File, FileLength, PixivPostHeader.ArtistName)
{
    public override string PostHtmlCaption =>
        $"<a href=\"https://www.pixiv.net/member_illust.php?mode=medium&illust_id={PostHeader.Id}\">pixiv</a>";
}
