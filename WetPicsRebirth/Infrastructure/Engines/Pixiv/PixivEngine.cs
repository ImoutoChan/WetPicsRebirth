using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WetPicsRebirth.Infrastructure.Engines.Pixiv.Models;
using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv
{
    public class PixivEngine : IPopularListLoaderEngine
    {
        private readonly HttpClient _httpClient;
        private readonly IPixivApiClient _pixivApiClient;

        public PixivEngine(HttpClient httpClient, IPixivApiClient pixivApiClient)
        {
            _httpClient = httpClient;
            _pixivApiClient = pixivApiClient;
        }

        public async Task<IReadOnlyCollection<PostHeader>> LoadPopularList(string options)
        {
            if (!Enum.TryParse(options, out PixivTopType pixivTopType))
                throw new ArgumentException($"Unable to parse pixiv options {options}", nameof(options));

            return await _pixivApiClient.LoadTop(pixivTopType);
        }

        public async Task<Post> LoadPost(PostHeader postHeader)
        {
            if (postHeader is PixivPostHeader pixivPostHeader)
            {
                var file = await _pixivApiClient.DownloadImage(pixivPostHeader.ImageUrl);

                return new PixivPost(pixivPostHeader, file);
            }

            throw new Exception("Wrong post header type");
        }
    }
}
