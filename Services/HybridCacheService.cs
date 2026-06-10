using library_management.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

public interface IHybridCacheService
{
    public Task SetEntry<T>(string key, T data);

    public Task<T?> GetEntry<T>(string key);

    public Task RemoveEntry(string key);
}

public class HybridCacheService : IHybridCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IRedisCacheService _distributedCache;
    public HybridCacheService(
        IMemoryCache memoryCache,
        IRedisCacheService redisCacheService
    )
    {
        _memoryCache = memoryCache;
        _distributedCache = redisCacheService;
    }

    public async Task SetEntry<T>(string key, T data)
    {
        _memoryCache.Set(key, data, AppCacheOptions.InMemoryCacheOptions);
        await _distributedCache.SetEntry(key, data);
    }

    public async Task<T> GetEntry<T>(string key)
    {
        if (_memoryCache.TryGetValue(key, out T data))
            return data;

        data = await _distributedCache.GetEntry<T>(key);

        return data;
    }

    public async Task RemoveEntry(string key)
    {
        _memoryCache.Remove(key);
        await _distributedCache.RemoveEntry(key);
    }
}