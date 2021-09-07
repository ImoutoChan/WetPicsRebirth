using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Imouto.BooruParser.Loaders;
using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Infrastructure.Engines
{
    public class BooruEngine : IPopularListLoaderEngine
    {
        private readonly IBooruAsyncLoader _loader;
        private readonly HttpClient _httpClient;

        public BooruEngine(IBooruAsyncLoader booruAsyncLoader, HttpClient httpClient)
        {
            _loader = booruAsyncLoader;
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<PostHeader>> LoadPopularList(string options)
        {
            var popularType = options switch
            {
                "month" => PopularType.Month,
                "week" => PopularType.Week,
                "day" => PopularType.Day,
                _ => PopularType.Day
            };

            var popular = await _loader.LoadPopularAsync(popularType);

            return popular.Results.Select(x => new PostHeader(x.Id, x.Md5)).ToList();
        }

        public async Task<Post> LoadPost(PostHeader postHeader)
        {
            var post = await _loader.LoadPostAsync(postHeader.Id);
            var bytes = await _httpClient.GetByteArrayAsync(post.OriginalUrl);

            return new Post(postHeader, post.OriginalUrl, bytes);
        }
    }
}
