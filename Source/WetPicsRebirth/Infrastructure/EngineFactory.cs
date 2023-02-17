using Flurl.Http.Configuration;
using Imouto.BooruParser.Implementations.Danbooru;
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

    public EngineFactory(
        HttpClient httpClient,
        IOptions<DanbooruConfiguration> danbooruConfiguration,
        IPixivApiClient pixivApiClient)
    {
        _httpClient = httpClient;
        _pixivApiClient = pixivApiClient;
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
                _httpClient),
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
                _httpClient),
            ImageSource.Pixiv => new PixivEngine(_pixivApiClient),
            _ => throw new ArgumentOutOfRangeException(nameof(imageSource), imageSource, null)
        };
    }
}
