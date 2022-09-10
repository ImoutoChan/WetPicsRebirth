using Microsoft.Extensions.Caching.Memory;
using WetPicsRebirth.Data.Entities;
using WetPicsRebirth.Extensions;
using WetPicsRebirth.Infrastructure.Models;

namespace WetPicsRebirth.Infrastructure;

public class PopularListLoader : IPopularListLoader
{
    private readonly IEngineFactory _engineFactory;
    private readonly IMemoryCache _memoryCache;

    public PopularListLoader(IEngineFactory engineFactory, IMemoryCache memoryCache)
    {
        _engineFactory = engineFactory;
        _memoryCache = memoryCache;
    }

    public Task<IReadOnlyCollection<PostHeader>> Load(ImageSource source, string options)
    {
        return _memoryCache.GetRequiredOrCreateAsync(source + options, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return _engineFactory.Get(source).LoadPopularList(options);
        });
    }

    public Task<LoadedPost> LoadPost(ImageSource source, PostHeader header)
    {
        return _engineFactory.Get(source).LoadPost(header);
    }

    public string CreateCaption(ImageSource source, string options, Post post)
    {
        return _engineFactory.Get(source).CreateCaption(source, options, post);
    }
}
