using System.Security.Cryptography;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Extensions;
using WetPicsRebirth.Infrastructure;
using WetPicsRebirth.Infrastructure.Engines;
using WetPicsRebirth.Infrastructure.Engines.Pixiv;
using WetPicsRebirth.Infrastructure.Engines.Pixiv.Models;
using Xunit;

namespace WetPicsRebirth.Tests;

public class PopularListLoaderTests
{
    private readonly PopularListLoader _loader;

    public PopularListLoaderTests()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", false).Build();

        var pixivOptions = Options.Create(config.GetSection("Pixiv").GetRequired<PixivConfiguration>());
        var danbooruOptions = Options.Create(config.GetSection("Danbooru").GetRequired<DanbooruConfiguration>());

        danbooruOptions.Value.BotUserAgent = "Tests/v1";
        
        _loader = new(
            new EngineFactory(
                new(),
                danbooruOptions,
                new PixivApiClient(
                    new(),
                    pixivOptions,
                    new PixivAuthorization(pixivOptions,
                        new MemoryCache(Options.Create(new MemoryCacheOptions())))),
                new NullLoggerFactory()),
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
        var post = await _loader.LoadPost(ImageSource.Yandere, new(666626, null));

        using MD5 md5 = MD5.Create();
        var hash = string.Join("", (await md5.ComputeHashAsync(post.Post.File)).Select(x => x.ToString("X2")));

        hash.ToUpperInvariant().Should().Be("4cedfc7918c5bf7b82ae7af1402ce9b7".ToUpperInvariant());
    }

    [Fact]
    public async Task DanbooruLoaderShouldLoadPost()
    {
        var post = await _loader.LoadPost(ImageSource.Danbooru, new(4695839, null));

        using MD5 md5 = MD5.Create();
        var hash = string.Join("", (await md5.ComputeHashAsync(post.Post.File)).Select(x => x.ToString("X2")));

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
                "腿之助兵衛",
                Array.Empty<string>()));

        using MD5 md5 = MD5.Create();
        var hash = string.Join("", (await md5.ComputeHashAsync(post.Post.File)).Select(x => x.ToString("X2")));

        hash.ToUpperInvariant().Should().Be("09109EE12C056E7457080B11067088D8".ToUpperInvariant());
    }

    [Fact]
    public async Task PostWithoutLoliShouldNotRequestModeration()
    {
        var post = await _loader.LoadPost(
            ImageSource.Pixiv,
            new PixivPostHeader(
                92561568,
                "https://i.pximg.net/img-original/img/2021/09/06/22/46/24/92561568_p0.jpg",
                "ミソラ　逆レ",
                "腿之助兵衛",
                Array.Empty<string>()));

        post.RequireModeration.Should().BeFalse();
    }

    [Theory]
    [InlineData("loli")]
    [InlineData("mini loli with breasts")]
    public async Task PostWithLoliShouldRequestModeration(string tag)
    {
        var pixivPostHeaderFaker = new Faker<PixivPostHeader>()
            .CustomInstantiator(x => new PixivPostHeader(
                92561568,
                x.Image.PicsumUrl(),
                x.Internet.UserName(),
                x.Name.FullName(),
                new[] { tag }));
        
        var post = await _loader.LoadPost(
            ImageSource.Pixiv,
            pixivPostHeaderFaker.Generate());

        post.RequireModeration.Should().BeTrue();
    }
}
