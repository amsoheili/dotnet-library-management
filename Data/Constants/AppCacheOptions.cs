using Microsoft.Extensions.Caching.Memory;

public static class AppCacheOptions
{
    public static readonly MemoryCacheEntryOptions InMemoryCacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                .SetSlidingExpiration(TimeSpan.FromSeconds(30));
}