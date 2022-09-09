using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Infrastructure.Models;

public record Post(PostHeader PostHeader, string Url, Stream File, long FileSize)
{
    public MediaType Type =>
        Url.EndsWith(".mp4") || Url.EndsWith(".webm") || Url.EndsWith(".swf")
            ? MediaType.Video
            : MediaType.Photo;

    public virtual string PostHtmlCaption => Url.Contains("yande.re")
        ? $"<a href=\"https://yande.re/post/show/{PostHeader.Id}\">yande.re</a>"
        : $"<a href=\"https://danbooru.donmai.us/posts/{PostHeader.Id}\">danbooru.donmai.us</a>";
}
