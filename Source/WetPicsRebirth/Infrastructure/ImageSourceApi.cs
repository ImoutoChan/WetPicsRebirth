using Flurl.Http.Configuration;
using Imouto.BooruParser;
using Imouto.BooruParser.Implementations.Danbooru;
using Imouto.BooruParser.Implementations.Yandere;
using Microsoft.Extensions.Options;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Infrastructure.Engines;

namespace WetPicsRebirth.Infrastructure;

public class ImageSourceApi : IImageSourceApi
{
    private readonly IFlurlClientCache _flurlClientCache;
    private readonly DanbooruConfiguration _danbooruConfiguration;
    private readonly ILogger<ImageSourceApi> _logger;

    public ImageSourceApi(
        IOptions<DanbooruConfiguration> danbooruConfiguration,
        IFlurlClientCache flurlClientCache,
        ILogger<ImageSourceApi> logger)
    {
        _flurlClientCache = flurlClientCache;
        _logger = logger;
        _danbooruConfiguration = danbooruConfiguration.Value;
    }

    public async Task FavoritePost(UserAccount account, int postId)
    {
        IBooruApiAccessor accessor = account switch
        {
            { Source: ImageSource.Danbooru } =>
                new DanbooruApiLoader(
                    _flurlClientCache,
                    Options.Create(new DanbooruSettings
                    {
                        ApiKey = account.ApiKey,
                        Login = account.Login,
                        PauseBetweenRequestsInMs = 0,
                        BotUserAgent = _danbooruConfiguration.BotUserAgent
                    })),
            { Source: ImageSource.Yandere } => 
                new YandereApiLoader(
                    _flurlClientCache,
                    Options.Create(new YandereSettings
                    {
                        Login = account.Login,
                        PasswordHash = account.ApiKey,
                        PauseBetweenRequestsInMs = 0,
                        BotUserAgent = _danbooruConfiguration.BotUserAgent
                    })),
            _ => throw new ArgumentOutOfRangeException(nameof(account))
        };

        _logger.LogInformation("Trying to favorite post {PostId} in {Source}", postId, account.Source);
        await accessor.FavoritePostAsync(postId);
        _logger.LogInformation("Post {PostId} in {Source} was favorited", postId, account.Source);
    }
}
