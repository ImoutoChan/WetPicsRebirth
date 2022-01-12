using Imouto.BooruParser.Loaders;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Infrastructure;

public class ImageSourceApi : IImageSourceApi
{
    private readonly HttpClient _httpClient;

    public ImageSourceApi(HttpClient httpClient) => _httpClient = httpClient;

    public Task FavoritePost(UserAccount account, int postId)
    {
        IBooruApiAccessor loader = account switch
        {
            { Source: ImageSource.Danbooru } acc => new DanbooruLoader(acc.Login, acc.ApiKey, 1000, _httpClient),
            { Source: ImageSource.Yandere } acc => new YandereLoader(_httpClient, login: acc.Login, passwordHash: acc.ApiKey),
            _ => throw new ArgumentOutOfRangeException(nameof(account))
        };

        return loader.FavoritePostAsync(postId);
    }
}
