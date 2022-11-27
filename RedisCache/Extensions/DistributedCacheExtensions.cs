using Microsoft.Extensions.Caching.Distributed;

namespace RedisCache.Extensions;

public static class DistributedCacheExtensions
{
    public static async Task<string> GetOrCreateStringAsync(this IDistributedCache cache, string key, Func<DistributedCacheEntryOptions, Task<string>> factory)
    {
        var entry = await cache.GetStringAsync(key);

        if (entry is null)
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            entry = await factory.Invoke(options);

            await cache.SetStringAsync(key, entry, options);
        }

        return entry;
    }
}