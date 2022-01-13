namespace WetPicsRebirth.Infrastructure.Models;

public record Post(PostHeader PostHeader, string Url, Stream File, long FileSize)
{
    public PostType Type =>
        Url.EndsWith(".mp4") || Url.EndsWith(".webm") || Url.EndsWith(".swf")
            ? PostType.Video
            : PostType.Image;
}

public enum PostType
{
    Image,
    Video
}
