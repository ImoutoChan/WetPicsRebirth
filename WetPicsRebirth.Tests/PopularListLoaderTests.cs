using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Infrastructure;
using WetPicsRebirth.Infrastructure.Engines;
using WetPicsRebirth.Infrastructure.Engines.Pixiv;
using WetPicsRebirth.Infrastructure.Engines.Pixiv.Models;
using WetPicsRebirth.Infrastructure.Models;
using Xunit;

namespace WetPicsRebirth.Tests;

public class PopularListLoaderTests
{
    private readonly PopularListLoader _loader;

    public PopularListLoaderTests()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", false).Build();

        var pixivOptions = Options.Create(config.GetSection("Pixiv").Get<PixivConfiguration>());
        var danbooruOptions = Options.Create(config.GetSection("Danbooru").Get<DanbooruConfiguration>());

        _loader = new PopularListLoader(
            new EngineFactory(
                new HttpClient(),
                danbooruOptions,
                new PixivApiClient(
                    new HttpClient(),
                    pixivOptions,
                    new PixivAuthorization(pixivOptions,
                        new MemoryCache(Options.Create(new MemoryCacheOptions()))))),
            new MemoryCache(Options.Create(new MemoryCacheOptions())));
    }

    [Theory]
    [InlineData("month")]
    [InlineData("week")]
    [InlineData("day")]
    public async Task YandereLoaderShouldLoadMonthList(string type)
    {
        var top = await _loader.Load(ImageSource.Yandere, type);

        top.Count.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData("month")]
    [InlineData("week")]
    [InlineData("day")]
    public async Task DanbooruLoaderShouldLoadMonthList(string type)
    {
        var top = await _loader.Load(ImageSource.Danbooru, type);

        top.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task PixivLoaderShouldLoadMonthList()
    {
        var top = await _loader.Load(ImageSource.Pixiv, "ByMaleR18");

        top.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task PixivShouldAuthorizeOnlyOnce()
    {
        var top1 = await _loader.Load(ImageSource.Pixiv, "ByMaleR18");
        var top2 = await _loader.Load(ImageSource.Pixiv, "Monthly");

        top1.Count.Should().BeGreaterThan(0);
        top2.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task YandereLoaderShouldLoadPost()
    {
        var post = await _loader.LoadPost(ImageSource.Yandere, new PostHeader(666626, null));

        using MD5 md5 = MD5.Create();
        var hash = string.Join("", md5.ComputeHash(post.File).Select(x => x.ToString("X2")));

        hash.ToUpperInvariant().Should().Be("4cedfc7918c5bf7b82ae7af1402ce9b7".ToUpperInvariant());
    }

    [Fact]
    public async Task DanbooruLoaderShouldLoadPost()
    {
        var post = await _loader.LoadPost(ImageSource.Danbooru, new PostHeader(4695839, null));

        using MD5 md5 = MD5.Create();
        var hash = string.Join("", md5.ComputeHash(post.File).Select(x => x.ToString("X2")));

        hash.ToUpperInvariant().Should().Be("44727b73a7995a630194cecf7d20e73a".ToUpperInvariant());
    }

    [Fact]
    public async Task PixivLoaderShouldLoadPost()
    {
        var post = await _loader.LoadPost(
            ImageSource.Pixiv,
            new PixivPostHeader(
                92561568,
                "https://i.pximg.net/img-original/img/2021/09/06/22/46/24/92561568_p0.jpg",
                "ミソラ　逆レ",
                "腿之助兵衛"));

        using MD5 md5 = MD5.Create();
        var hash = string.Join("", md5.ComputeHash(post.File).Select(x => x.ToString("X2")));

        hash.ToUpperInvariant().Should().Be("09109EE12C056E7457080B11067088D8".ToUpperInvariant());
    }
}