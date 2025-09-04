using Flurl.Http.Configuration;
using Imouto.BooruParser.Implementations.Danbooru;
using Imouto.BooruParser.Implementations.Gelbooru;
using Imouto.BooruParser.Implementations.Rule34;
using Imouto.BooruParser.Implementations.Yandere;
using Microsoft.Extensions.Options;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Infrastructure.Engines;
using WetPicsRebirth.Infrastructure.Engines.Pixiv;
using IHttpClientFactory = System.Net.Http.IHttpClientFactory;

namespace WetPicsRebirth.Infrastructure;

public class EngineFactory : IEngineFactory
{
    private readonly DanbooruConfiguration _danbooruConfiguration;
    private readonly HttpClient _httpClient;
    private readonly IPixivApiClient _pixivApiClient;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFlurlClientCache _flurlClientCache;

    public EngineFactory(
        HttpClient httpClient,
        IOptions<DanbooruConfiguration> danbooruConfiguration,
        IPixivApiClient pixivApiClient,
        ILoggerFactory loggerFactory,
        IHttpClientFactory httpClientFactory,
        IFlurlClientCache flurlClientCache)
    {
        _httpClient = httpClient;
        _pixivApiClient = pixivApiClient;
        _loggerFactory = loggerFactory;
        _httpClientFactory = httpClientFactory;
        _flurlClientCache = flurlClientCache;
        _danbooruConfiguration = danbooruConfiguration.Value;
    }

    public IPopularListLoaderEngine Get(ImageSource imageSource)
    {
        var danbooruHttpClient = _httpClientFactory.CreateClient();
        danbooruHttpClient.DefaultRequestHeaders.Add("User-Agent", _danbooruConfiguration.BotUserAgent);
        
        return imageSource switch
        {
            ImageSource.Yandere => new BooruEngine(
                new YandereApiLoader(
                    _flurlClientCache,
                    Options.Create(new YandereSettings()
                    {
                        BotUserAgent = _danbooruConfiguration.BotUserAgent
                    })),
                _httpClient,
                _loggerFactory.CreateLogger<BooruEngine>()),
            ImageSource.Danbooru => new FallbackToGelbooruDanbooruEngine(
                new DanbooruApiLoader(
                    _flurlClientCache,
                    Options.Create(new DanbooruSettings()
                    {
                        ApiKey = _danbooruConfiguration.ApiKey,
                        Login = _danbooruConfiguration.Username,
                        PauseBetweenRequestsInMs = _danbooruConfiguration.Delay,
                        BotUserAgent = _danbooruConfiguration.BotUserAgent
                    })),
                new GelbooruApiLoader(
                    _flurlClientCache,
                    Options.Create(new GelbooruSettings()
                    {
                        PauseBetweenRequestsInMs = _danbooruConfiguration.Delay,
                        ApiKey = "",
                        UserId = 0
                    })),
                danbooruHttpClient,
                _loggerFactory.CreateLogger<FallbackToGelbooruDanbooruEngine>()),
            ImageSource.Rule34 => new BooruEngine(
                new Rule34ApiLoader(
                    _flurlClientCache,
                    Options.Create(new Rule34Settings()
                    {
                        PauseBetweenRequestsInMs = 1,
                        ApiKey = "",
                        UserId = 0
                    })),
                _httpClient,
                _loggerFactory.CreateLogger<BooruEngine>()),
            ImageSource.Gelbooru => new BooruEngine(
                new GelbooruApiLoader(
                    _flurlClientCache,
                    Options.Create(new GelbooruSettings()
                    {
                        PauseBetweenRequestsInMs = 1,
                        ApiKey = "",
                        UserId = 0
                    })),
                _httpClient,
                _loggerFactory.CreateLogger<BooruEngine>()),
            ImageSource.Pixiv => new PixivEngine(_pixivApiClient),
            _ => throw new ArgumentOutOfRangeException(nameof(imageSource), imageSource, null)
        };
    }
}
