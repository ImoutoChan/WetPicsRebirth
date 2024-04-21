using System.Net;
using Imouto.BooruParser;
using WetPicsRebirth.Infrastructure.Models;
using Post = WetPicsRebirth.Infrastructure.Models.Post;

namespace WetPicsRebirth.Infrastructure.Engines;

/// <summary>
/// This is temporary fix for blocked bots in Danbooru. 
/// </summary>
public class FallbackToGelbooruDanbooruEngine : BooruEngine
{
    private readonly IBooruApiLoader _gelbooruLoader;
    private readonly HttpClient _httpClient;
    private readonly ILogger<FallbackToGelbooruDanbooruEngine> _logger;

    public FallbackToGelbooruDanbooruEngine(
        IBooruApiLoader danbooruLoader,
        IBooruApiLoader gelbooruLoader,
        HttpClient httpClient,
        ILogger<FallbackToGelbooruDanbooruEngine> logger) 
        : base(danbooruLoader, httpClient, logger)
    {
        _gelbooruLoader = gelbooruLoader;
        _httpClient = httpClient;
        _logger = logger;
    }

    public override async Task<LoadedPost> LoadPost(PostHeader postHeader)
    {
        try
        {
            return await base.LoadPost(postHeader);
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.Forbidden)
        {
            // ignore
        }
        
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
            if (postHeader.Md5Hash == null)
                throw new InvalidOperationException("Md5 hash is null");
            
            var post = await _gelbooruLoader.GetPostByMd5Async(postHeader.Md5Hash!);
            
            if (post == null)
                throw new InvalidOperationException($"Gelbooru post with md5: {postHeader.Md5Hash} not found");
            
            mediaUrl = GetMediaUrl(post);

            var response = await _httpClient.GetAsync(mediaUrl);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var length = response.Content.Headers.ContentLength;

            if (!length.HasValue)
                throw new("Unexpected length");

            var artist = post.Tags.FirstOrDefault(x => x.Type == "artist")?.Name;

            var resultPost = new Post(postHeader, mediaUrl, stream, length.Value, artist);
            var requireModeration = CheckForModeration(post);

            return new(resultPost, requireModeration);
        }
    }

    private static string GetMediaUrl(Imouto.BooruParser.Post post)
        => post.OriginalUrl?.EndsWith(".zip") == true ? post.SampleUrl! : post.OriginalUrl!;

    private static bool CheckForModeration(Imouto.BooruParser.Post post)
        => post.Rating == Rating.Explicit && post.Tags.Any(x => x.Name is "loli" or "shota");
}
