using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using WetPicsRebirth.Extensions;
using WetPicsRebirth.Infrastructure.Engines.Pixiv.Dto;
using WetPicsRebirth.Infrastructure.Engines.Pixiv.Models;

namespace WetPicsRebirth.Infrastructure.Engines.Pixiv
{
    public class PixivApiClient : IPixivApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<PixivConfiguration> _configuration;
        private readonly IPixivAuthorization _pixivAuthorization;

        public PixivApiClient(
            HttpClient httpClient,
            IOptions<PixivConfiguration> configuration,
            IPixivAuthorization pixivAuthorization)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _pixivAuthorization = pixivAuthorization;
        }

        public async Task<IReadOnlyCollection<PixivPostHeader>> LoadTop(PixivTopType topType, int page = 1, int count = 100)
        {
            const string url = "https://public-api.secure.pixiv.net/v1/ranking/all";

            var param = new Dictionary<string, string>
            {
                {"mode", topType.GetEnumDescription()},
                {"page", page.ToString()},
                {"per_page", count.ToString()},
                {"include_stats", "1"},
                {"include_sanity_level", "1"},
                {"image_sizes", "large"},
                {"profile_image_sizes", "px_50x50"}
            };

            var responseContent = await GetAsync(url, param);
            var rank = JToken.Parse(responseContent).SelectToken("response")?.ToObject<IReadOnlyCollection<Rank>>()
                ?.FirstOrDefault();

            if (rank == null)
                throw new Exception($"Unexpected pixiv response: {responseContent}");

            return rank.Works.Select(x => x.Work)
                .Select(x => new PixivPostHeader((int)x.Id!.Value, x.ImageUrls.Large, x.Title, x.User.Name))
                .ToList();

        }

        private async Task<string> GetAsync(
            string url,
            IDictionary<string, string> param,
            bool isRetry = false)
        {
            var token = await _pixivAuthorization.GetAccessToken();

            url += "?" + string.Join("&", param.Select(x => x.Key + "=" + WebUtility.UrlEncode(x.Value)));

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Referrer = new Uri("http://spapi.pixiv.net/");
            request.Headers.UserAgent.Clear();
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("PixivIOSApp", "5.8.0"));
            request.Headers.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);

            try
            {
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch when(!isRetry)
            {
                _pixivAuthorization.ResetAccessToken();
                return await GetAsync(url, param, true);
            }
        }

        public async Task<Stream> DownloadImage(string imageUrl)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, imageUrl);

            request.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 6.0; zh-CN; rv:1.9.0.6) Gecko/2009011913 Firefox/3.0.6");
            request.Headers.TryAddWithoutValidation("Accept-Language", "zh-cn,zh;q=0.7,ja;q=0.3");
            request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip,deflate");
            request.Headers.TryAddWithoutValidation("Accept-Charset", "gb18030,utf-8;q=0.7,*;q=0.7");
            request.Headers.TryAddWithoutValidation("referer", "http://www.pixiv.net/member_illust.php?mode=manga&illust_id=38889015");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
