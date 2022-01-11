using Microsoft.Extensions.Caching.Memory;
using WetPicsRebirth.Data.Entities;

namespace WetPicsRebirth.Data.Repositories;

internal class CachedUserAccountsRepository : IUserAccountsRepository
{
    private readonly UserAccountsRepository _repository;
    private readonly IMemoryCache _memoryCache;

    public CachedUserAccountsRepository(UserAccountsRepository repository, IMemoryCache memoryCache)
    {
        _repository = repository;
        _memoryCache = memoryCache;
    }

    public async Task<UserAccount> Set(long userId, ImageSource source, string login, string apikey)
    {
        var item = await _repository.Set(userId, source, login, apikey);
        _memoryCache.Set(GetKey(userId, source), item);

        return item;
    }

    public Task<UserAccount?> Get(long userId, ImageSource source)
        => _memoryCache.GetOrCreateAsync(GetKey(userId, source), _ => _repository.Get(userId, source));

    private static string GetKey(long userId, ImageSource source) => $"user_account_{userId}_{source}";
}