using Flurl.Http.Configuration;
using Imouto.BooruParser.Implementations.Danbooru;
using Imouto.BooruParser.Implementations.Gelbooru;
using Imouto.BooruParser.Implementations.Rule34;
using Imouto.BooruParser.Implementations.Yandere;
using Microsoft.Extensions.Options;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Infrastructure.Engines;
using WetPicsRebirth.Infrastructure.Engines.Pixiv;

namespace WetPicsRebirth.Infrastructure;

public class EngineFactory : IEngineFactory
{
    private readonly DanbooruConfiguration _danbooruConfiguration;
    private readonly HttpClient _httpClient;
    private readonly IPixivApiClient _pixivApiClient;
    private readonly ILoggerFactory _loggerFactory;

    public EngineFactory(
        HttpClient httpClient,
        IOptions<DanbooruConfiguration> danbooruConfiguration,
        IPixivApiClient pixivApiClient,
        ILoggerFactory loggerFactory)
    {
        _httpClient = httpClient;
        _pixivApiClient = pixivApiClient;
        _loggerFactory = loggerFactory;
        _danbooruConfiguration = danbooruConfiguration.Value;
    }

    public IPopularListLoaderEngine Get(ImageSource imageSource)
    {
        return imageSource switch
        {
            ImageSource.Yandere => new BooruEngine(
                new YandereApiLoader(
                    new PerBaseUrlFlurlClientFactory(),
                    Options.Create(new YandereSettings())), 
                _httpClient,
                _loggerFactory.CreateLogger<BooruEngine>()),
            ImageSource.Danbooru => new BooruEngine(
                new DanbooruApiLoader(
                    new PerBaseUrlFlurlClientFactory(),
                    Options.Create(new DanbooruSettings()
                    {
                        ApiKey = _danbooruConfiguration.ApiKey,
                        Login = _danbooruConfiguration.Username,
                        PauseBetweenRequestsInMs = _danbooruConfiguration.Delay,
                        BotUserAgent = _danbooruConfiguration.BotUserAgent
                    })),
                _httpClient,
                _loggerFactory.CreateLogger<BooruEngine>()),
            ImageSource.Rule34 => new BooruEngine(
                new Rule34ApiLoader(
                    new PerBaseUrlFlurlClientFactory(),
                    Options.Create(new Rule34Settings()
                    {
                        PauseBetweenRequestsInMs = 1,
                    })),
                _httpClient,
                _loggerFactory.CreateLogger<BooruEngine>()),
            ImageSource.Gelbooru => new BooruEngine(
                new GelbooruApiLoader(
                    new PerBaseUrlFlurlClientFactory(),
                    Options.Create(new GelbooruSettings()
                    {
                        PauseBetweenRequestsInMs = 1,
                    })),
                _httpClient,
                _loggerFactory.CreateLogger<BooruEngine>()),
            ImageSource.Pixiv => new PixivEngine(_pixivApiClient),
            _ => throw new ArgumentOutOfRangeException(nameof(imageSource), imageSource, null)
        };
    }
}
