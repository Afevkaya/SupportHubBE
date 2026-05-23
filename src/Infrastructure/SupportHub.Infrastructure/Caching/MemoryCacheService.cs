using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Caching;

namespace SupportHub.Infrastructure.Caching;

public class MemoryCacheService(
    IMemoryCache memoryCache,
    ILogger<MemoryCacheService> logger) : ICacheService
{
    private readonly HashSet<string> _cacheKeys = [];
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (memoryCache.TryGetValue(key, out T? value))
        {
            logger.LogInformation("Cache hit. CacheKey: {CacheKey}", key);
            return Task.FromResult(value);
        }

        logger.LogInformation("Cache miss. CacheKey: {CacheKey}", key);
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        memoryCache.Set(key, value, expiration);
        _cacheKeys.Add(key);
        logger.LogInformation("Cache entry added. CacheKey: {CacheKey}, Expiration: {Expiration}", key, expiration);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var keysToRemove = _cacheKeys.Where(k => k.StartsWith(prefix)).ToList();
        foreach (var key in keysToRemove)
        {
            memoryCache.Remove(key);
            _cacheKeys.Remove(key);
            logger.LogInformation("Cache invalidated. CacheKey: {CacheKey}, Prefix: {Prefix}", key, prefix);
        }
        return Task.CompletedTask;
    }
}