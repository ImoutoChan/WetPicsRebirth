using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Imouto.BooruParser.Loaders;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Extensions;
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
            var popularType = GetPopularType(options);

            var popular = await _loader.LoadPopularAsync(popularType);

            return popular.Results.Select(x => new PostHeader(x.Id, x.Md5)).ToList();
        }

        public async Task<Post> LoadPost(PostHeader postHeader)
        {
            var post = await _loader.LoadPostAsync(postHeader.Id);
            var file = await _httpClient.GetStreamAsync(post.OriginalUrl);

            return new Post(postHeader, post.OriginalUrl, file);
        }

        public string CreateCaption(ImageSource source, string options, Post post)
        {
            var type = GetPopularType(options).MakeAdverb().ToLower();

            return source switch
            {
                ImageSource.Danbooru => $"<a href=\"https://danbooru.donmai.us/posts/{post.PostHeader.Id}\">danbooru {type}</a>",
                ImageSource.Yandere => $"<a href=\"https://yande.re/post/show/{post.PostHeader.Id}\">yandere {type}</a>",
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
    }
}
