using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WetPicsRebirth.Data.Entities;
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
            if (!Enum.TryParse(options, true, out PixivTopType pixivTopType))
                throw new ArgumentException($"Unable to parse pixiv options {options}", nameof(options));

            return await _pixivApiClient.LoadTop(pixivTopType);
        }

        public async Task<Post> LoadPost(PostHeader postHeader)
        {
            if (postHeader is not PixivPostHeader pixivPostHeader)
                throw new Exception("Wrong post header type");

            var file = await _pixivApiClient.DownloadImage(pixivPostHeader.ImageUrl);

            return new PixivPost(pixivPostHeader, file);
        }

        public string CreateCaption(ImageSource source, string options, Post post)
        {
            if (!Enum.TryParse(options, true, out PixivTopType type) || post.PostHeader is not PixivPostHeader header)
                throw new ArgumentException($"Unable to parse pixiv options {options}", nameof(options));

            return $"<a href=\"https://www.pixiv.net/member_illust.php?mode=medium&illust_id={post.PostHeader.Id}\">" +
                   $"Pixiv {type.ToString()} # {EscapeHtml(header.Title)} © {EscapeHtml(header.ArtistName)}</a>";
        }

        private static string EscapeHtml(string input)
            => input
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("&", "&amp;");
    }
}
