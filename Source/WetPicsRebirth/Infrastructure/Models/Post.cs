using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Infrastructure.Models;

public record Post(PostHeader PostHeader, string Url, Stream File, long FileSize, string? Author)
{
    public MediaType Type =>
        Url.EndsWith(".mp4") || Url.EndsWith(".webm") || Url.EndsWith(".swf")
            ? MediaType.Video
            : MediaType.Photo;

    public string FileName => Path.GetFileName(Url);

    public virtual string PostHtmlCaption => Url.Contains("yande.re")
        ? $"<a href=\"https://yande.re/post/show/{PostHeader.Id}\">yande.re</a>"
        : $"<a href=\"https://danbooru.donmai.us/posts/{PostHeader.Id}\">danbooru.donmai.us</a>";
}

public record BannedPost(PostHeader PostHeader) : Post(PostHeader, string.Empty, Stream.Null, 0, null);
