using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Caching;

namespace SupportHub.Infrastructure.Caching;

public class MemoryCacheService(
    IMemoryCache memoryCache,
    ILogger<MemoryCacheService> logger) : ICacheService
{
    private readonly ConcurrentDictionary<string, byte> _cacheKeys = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<T?>(cancellationToken);
        }

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
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        cacheOptions.RegisterPostEvictionCallback((evictedKey, _, _, _) =>
        {
            if (evictedKey is string cacheKey)
            {
                _cacheKeys.TryRemove(cacheKey, out _);
            }
        });

        memoryCache.Set(key, value, cacheOptions);
        _cacheKeys.TryAdd(key, 0);
        logger.LogInformation("Cache entry added. CacheKey: {CacheKey}, Expiration: {Expiration}", key, expiration);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        var keysToRemove = _cacheKeys.Keys
            .Where(k => k.StartsWith(prefix, StringComparison.Ordinal))
            .ToList();

        foreach (var key in keysToRemove)
        {
            memoryCache.Remove(key);
            _cacheKeys.TryRemove(key, out _);
            logger.LogInformation("Cache invalidated. CacheKey: {CacheKey}, Prefix: {Prefix}", key, prefix);
        }

        return Task.CompletedTask;
    }
}
