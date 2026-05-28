using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Caching;
using SupportHub.Application.Abstractions.Messaging;
using SupportHub.Application.Abstractions.Services;

namespace SupportHub.Application.Behaviors;

public class CachingBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ICurrentService currentService,
    ILogger<CachingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery cacheableQuery)
        {
            return await next(cancellationToken);
        }

        var cacheKey = cacheableQuery.GetCacheKey(currentService.UserId);

        if (await cacheService.GetAsync<TResponse>(cacheKey, cancellationToken) is TResponse cachedResponse)
        {
            logger.LogInformation("{RequestName} Cache hit for key {CacheKey}", typeof(TRequest).Name, cacheKey);
            return cachedResponse!;
        }

        logger.LogInformation("Cache miss for {RequestName}. CacheKey: {CacheKey}", typeof(TRequest).Name, cacheKey);

        var response = await next(cancellationToken);
        await cacheService.SetAsync(cacheKey, response, cacheableQuery.Expiration, cancellationToken);
        logger.LogInformation("{RequestName} Cache set for key {CacheKey} {Expiration}", typeof(TRequest).Name,
            cacheKey, cacheableQuery.Expiration);

        return response;
    }
}
