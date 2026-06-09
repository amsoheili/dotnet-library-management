using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace library_management.Services;

public interface IRedisCacheService
{
    public Task SetEntry<T>(string key, T data);

    public Task<T?> GetEntry<T>(string key);

    public Task RemoveEntry(string key);
}

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SetEntry<T>(string key, T data)
    {
        var json = JsonSerializer.Serialize(data);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };

        await _cache.SetStringAsync(key, json, options);
    }

    public async Task<T?> GetEntry<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);

        if (json is null)
            return default(T);

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task RemoveEntry(string key)
    {
        await _cache.RemoveAsync(key);
    }

}