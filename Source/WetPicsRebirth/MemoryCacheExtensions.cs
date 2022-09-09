using Microsoft.Extensions.Caching.Memory;

namespace WetPicsRebirth;

public static class MemoryCacheExtensions
{
    public static async Task<TItem> GetRequiredOrCreateAsync<TItem>(
        this IMemoryCache cache,
        object key,
        Func<ICacheEntry, Task<TItem>> factory)
    {
        if (cache.TryGetValue(key, out TItem? result)) 
            return result ?? throw new Exception("Extracted value is null");
        
        using var entry = cache.CreateEntry(key);

        result = await factory(entry);
        entry.Value = result;

        return result ?? throw new Exception("Extracted value is null");
    }
}