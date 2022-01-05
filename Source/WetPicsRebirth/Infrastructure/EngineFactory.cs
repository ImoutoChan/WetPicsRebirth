using Imouto.BooruParser.Loaders;
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
            ImageSource.Yandere => new BooruEngine(new YandereLoader(_httpClient), _httpClient),
            ImageSource.Danbooru => new BooruEngine(
                new DanbooruLoader(
                    _danbooruConfiguration.Username,
                    _danbooruConfiguration.ApiKey,
                    _danbooruConfiguration.Delay,
                    _httpClient),
                _httpClient),
            ImageSource.Pixiv => new PixivEngine(_httpClient, _pixivApiClient),
            _ => throw new ArgumentOutOfRangeException(nameof(imageSource), imageSource, null)
        };
    }
}