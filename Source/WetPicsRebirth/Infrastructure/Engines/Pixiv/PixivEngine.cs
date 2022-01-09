using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Infrastructure.Engines.Pixiv.Models;
using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv;

public class PixivEngine : IPopularListLoaderEngine
{
    private readonly IPixivApiClient _pixivApiClient;

    public PixivEngine(IPixivApiClient pixivApiClient)
    {
        _pixivApiClient = pixivApiClient;
    }

    public async Task<IReadOnlyCollection<PostHeader>> LoadPopularList(string options)
    {
        if (!Enum.TryParse(options, true, out PixivTopType pixivTopType))
            throw new ArgumentException($"Unable to parse pixiv options {options}", nameof(options));

        return await _pixivApiClient.LoadTop(pixivTopType);
    }

    public async Task<LoadedPost> LoadPost(PostHeader postHeader)
    {
        if (postHeader is not PixivPostHeader pixivPostHeader)
            throw new("Wrong post header type");

        var (stream, length) = await _pixivApiClient.DownloadImage(pixivPostHeader.ImageUrl);

        var resultPost = new PixivPost(pixivPostHeader, stream, length);
        var requireModeration = CheckForModeration(pixivPostHeader);

        return new(resultPost, requireModeration);
    }

    public string CreateCaption(ImageSource source, string options, Post post)
    {
        if (!Enum.TryParse(options, true, out PixivTopType type) || post.PostHeader is not PixivPostHeader header)
            throw new ArgumentException($"Unable to parse pixiv options {options}", nameof(options));

        return $"<a href=\"https://www.pixiv.net/member_illust.php?mode=medium&illust_id={post.PostHeader.Id}\">pixiv {options.ToLower()}</a>";
    }

    private static string EscapeHtml(string input)
        => input
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("&", "&amp;");

    private static bool CheckForModeration(PixivPostHeader header) =>
        header.Tags.Any(x => x.Contains("loli") || x.Contains("shota"));
}
