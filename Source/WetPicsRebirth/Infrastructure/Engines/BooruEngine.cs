using System.IO.Compression;
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
    private readonly ILogger<BooruEngine> _logger;

    public BooruEngine(IBooruApiLoader booruAsyncLoader, HttpClient httpClient, ILogger<BooruEngine> logger)
    {
        _loader = booruAsyncLoader;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<PostHeader>> LoadPopularList(string options)
    {
        var popularType = GetPopularType(options);

        var popular = await _loader.GetPopularPostsAsync(popularType);

        return popular.Results.Select(x => new PostHeader(x.Id, x.Md5Hash)).ToList();
    }

    public virtual async Task<LoadedPost> LoadPost(PostHeader postHeader)
    {
        var postId = postHeader.Id;
        var mediaUrl = string.Empty;
        
        try
        {
            return await LoadPostCore();
        }
        catch (Exception e) when(string.IsNullOrWhiteSpace(mediaUrl))
        {
            _logger.LogWarning(e, "Failed to load post {PostId} with media url {MediaUrl}", postId, mediaUrl);
            throw;
        }        
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load post {PostId} with media url {MediaUrl}", postId, mediaUrl);
            throw;
        }        
        
        async Task<LoadedPost> LoadPostCore()
        {
            var post = await _loader.GetPostAsync(postHeader.Id);
            mediaUrl = GetMediaUrl(post);

            if (string.IsNullOrWhiteSpace(mediaUrl))
            {
                return new LoadedPost(new BannedPost(postHeader));
            }

            var loaded = IsUgoira(post) 
                ? await LoadPostFromUgoira(post.OriginalUrl!) 
                : await LoadRegularPost(mediaUrl, post);

            var author = post.Tags.Where(x => x.Type == "artist").Select(x => x.Name).FirstOrDefault();
            
            var resultPost = new Post(postHeader, mediaUrl, loaded.Stream, loaded.Length, author);
            var requireModeration = CheckForModeration(post);

            return new(resultPost, requireModeration);
        }
    }

    private static bool IsUgoira(Imouto.BooruParser.Post post)
        => post.OriginalUrl?.EndsWith(".zip") == true;
    
    private async Task<(Stream Stream, long Length)> LoadRegularPost(string mediaUrl, Imouto.BooruParser.Post post)
    {
        try
        {
            var response = await _httpClient.GetAsync(mediaUrl);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var length = response.Content.Headers.ContentLength;

            if (!length.HasValue)
                throw new("Unexpected length");

            return (stream, length.Value);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load media from {MediaUrl} ({Id} {Md5}) ", mediaUrl, post.Id.Id, post.Id.Md5Hash);
            throw;
        }
    }

    private async Task<(Stream Stream, long Length)> LoadPostFromUgoira(string mediaUrl)
    {
        var response = await _httpClient.GetAsync(mediaUrl);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();

        using var ugoira = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
        var firstImage = ugoira.Entries.MinBy(x => x.Name);
        
        if (firstImage == null)
            throw new("No images in ugoira");

        var ms = new MemoryStream();
        await using var imageStream = firstImage.Open();
        await imageStream.CopyToAsync(ms);
        ms.Seek(0, SeekOrigin.Begin);

        return (ms, ms.Length);
    }

    private static string GetMediaUrl(Imouto.BooruParser.Post post)
    {
        if (post.OriginalUrl?.EndsWith(".zip") == true || post.OriginalUrl?.EndsWith(".webm") == true)
        {
            if (post.SampleUrl?.EndsWith(".webm") != true)
            {
                return post.SampleUrl!;
            }
            else if (post.PreviewUrl != null)
            {
                return post.PreviewUrl!;
            }
        }

        return post.OriginalUrl!;
    }

    public string CreateCaption(ImageSource source, string options, Post post)
    {
        var type = options == string.Empty 
            ? "selected" 
            : GetPopularType(options).MakeAdverb().ToLower();

        var postAuthor = post.Author is not null 
            ? $"{post.Author.Replace("_", " ")}" 
            : null;
        
        return source switch
        {
            ImageSource.Danbooru => $"<a href=\"https://danbooru.donmai.us/posts/{post.PostHeader.Id}\">{postAuthor ?? $"danbooru {type}"}</a>",
            ImageSource.Yandere => $"<a href=\"https://yande.re/post/show/{post.PostHeader.Id}\">{postAuthor ?? $"yandere {type}"}</a>",
            ImageSource.Rule34 => $"<a href=\"https://rule34.xxx/index.php?page=post&s=view&id={post.PostHeader.Id}\">{postAuthor ?? $"rule34 {type}"}</a>",
            ImageSource.Gelbooru => $"<a href=\"https://gelbooru.com/index.php?page=post&s=view&id={post.PostHeader.Id}\">{postAuthor ?? $"gelbooru {type}"}</a>",
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    public string CreateCaptionObsolete(ImageSource source, string options, Post post)
    {
        var type = options == string.Empty 
            ? "selected" 
            : GetPopularType(options).MakeAdverb().ToLower();

        return source switch
        {
            ImageSource.Danbooru => $"<a href=\"https://danbooru.donmai.us/posts/{post.PostHeader.Id}\">danbooru {type}</a>",
            ImageSource.Yandere => $"<a href=\"https://yande.re/post/show/{post.PostHeader.Id}\">yandere {type}</a>",
            ImageSource.Rule34 => $"<a href=\"https://rule34.xxx/index.php?page=post&s=view&id={post.PostHeader.Id}\">rule34 {type}</a>",
            ImageSource.Gelbooru => $"<a href=\"https://gelbooru.com/index.php?page=post&s=view&id={post.PostHeader.Id}\">gelbooru {type}</a>",
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
