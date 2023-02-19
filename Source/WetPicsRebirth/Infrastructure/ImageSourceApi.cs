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
    private readonly DanbooruConfiguration _danbooruConfiguration;

    public ImageSourceApi(IOptions<DanbooruConfiguration> danbooruConfiguration) 
        => _danbooruConfiguration = danbooruConfiguration.Value;

    public Task FavoritePost(UserAccount account, int postId)
    {
        IBooruApiAccessor accessor = account switch
        {
            { Source: ImageSource.Danbooru } =>
                new DanbooruApiLoader(
                    new PerBaseUrlFlurlClientFactory(), 
                    Options.Create(new DanbooruSettings()
                    {
                        ApiKey = account.ApiKey,
                        Login = account.Login,
                        PauseBetweenRequestsInMs = 1000,
                        BotUserAgent = _danbooruConfiguration.BotUserAgent
                    })),
            { Source: ImageSource.Yandere } => 
                new YandereApiLoader(
                    new PerBaseUrlFlurlClientFactory(), 
                    Options.Create(new YandereSettings()
                    {
                        Login = account.Login,
                        PasswordHash = account.ApiKey
                    })),
            _ => throw new ArgumentOutOfRangeException(nameof(account))
        };

        return accessor.FavoritePostAsync(postId);
    }
}
