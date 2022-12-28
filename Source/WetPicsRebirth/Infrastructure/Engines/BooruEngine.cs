using Imouto.BooruParser;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Extensions;
using WetPicsRebirth.Infrastructure.Models;
using Post = WetPicsRebirth.Infrastructure.Models.Post;

namespace WetPicsRebirth.Infrastructure.Engines;

public class BooruEngine : IPopularListLoaderEngine
{
    private readonly IBooruApiLoader _loader;
    private readonly HttpClient _httpClient;

    public BooruEngine(IBooruApiLoader booruAsyncLoader, HttpClient httpClient)
    {
        _loader = booruAsyncLoader;
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<PostHeader>> LoadPopularList(string options)
    {
        var popularType = GetPopularType(options);

        var popular = await _loader.GetPopularPostsAsync(popularType);

        return popular.Results.Select(x => new PostHeader(x.Id, x.Md5Hash)).ToList();
    }

    public async Task<LoadedPost> LoadPost(PostHeader postHeader)
    {
        var post = await _loader.GetPostAsync(postHeader.Id);
        var mediaUrl = GetMediaUrl(post);
        
        var response = await _httpClient.GetAsync(mediaUrl);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        var length = response.Content.Headers.ContentLength;

        if (!length.HasValue)
        {
            throw new("Unexpected length");
        }

        var resultPost = new Post(postHeader, mediaUrl, stream, length.Value);
        var requireModeration = CheckForModeration(post);

        return new(resultPost, requireModeration);
    }

    private static string GetMediaUrl(Imouto.BooruParser.Post post)
        => post.OriginalUrl?.EndsWith(".zip") == true ? post.SampleUrl! : post.OriginalUrl!;

    public string CreateCaption(ImageSource source, string options, Post post)
    {
        var type = GetPopularType(options).MakeAdverb().ToLower();

        return source switch
        {
            ImageSource.Danbooru => $"<a href=\"https://danbooru.donmai.us/posts/{post.PostHeader.Id}\">danbooru {type}</a>",
            ImageSource.Yandere => $"<a href=\"https://yande.re/post/show/{post.PostHeader.Id}\">yandere {type}</a>",
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    private static PopularType GetPopularType(string options)
    {
        var popularType = options switch
        {
            "month" => PopularType.Month,
            "week" => PopularType.Week,
            "day" => PopularType.Day,
            _ => PopularType.Day
        };
        return popularType;
    }

    private static bool CheckForModeration(Imouto.BooruParser.Post post)
        => post.Rating == Rating.Explicit && post.Tags.Any(x => x.Name is "loli" or "shota");
}
