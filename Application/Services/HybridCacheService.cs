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
    private readonly ILogger<HybridCacheService> _logger;
    public HybridCacheService(
        IMemoryCache memoryCache,
        IRedisCacheService redisCacheService,
        ILogger<HybridCacheService> logger
    )
    {
        _memoryCache = memoryCache;
        _distributedCache = redisCacheService;
        _logger = logger;
    }

    public async Task SetEntry<T>(string key, T data)
    {
        _memoryCache.Set(key, data, AppCacheOptions.InMemoryCacheOptions);

        _logger.LogInformation($"Memory cache with {key} key is set");

        await _distributedCache.SetEntry(key, data);

        _logger.LogInformation($"Distributed cache with {key} key is set");
    }

    public async Task<T> GetEntry<T>(string key)
    {
        if (_memoryCache.TryGetValue(key, out T data))
        {
            _logger.LogInformation($"Data with {key} key retrieved from memory cache");

            return data;
        }

        data = await _distributedCache.GetEntry<T>(key);

        if (data is not null)
        {
            _logger.LogInformation($"Data with {key} key retrieved from distributed cache");

            _memoryCache.Set(key, data, AppCacheOptions.InMemoryCacheOptions);

            _logger.LogInformation($"Memory cache with {key} key is set");
        }

        return data;
    }

    public async Task RemoveEntry(string key)
    {
        _memoryCache.Remove(key);
        await _distributedCache.RemoveEntry(key);
    }
}