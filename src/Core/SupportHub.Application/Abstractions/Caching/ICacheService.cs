namespace SupportHub.Application.Abstractions.Caching;

public interface ICacheService
{   
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default);

    Task RemoveByPrefixAsync(
        string prefix,
        CancellationToken cancellationToken = default);
}