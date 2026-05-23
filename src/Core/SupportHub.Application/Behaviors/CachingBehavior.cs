using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Caching;
using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Behaviors;

public class CachingBehavior<TRequest,TResponse>(
     ICacheService cacheService,
     ILogger<CachingBehavior<TRequest,TResponse>> logger) : IPipelineBehavior<TRequest,TResponse> where TRequest : notnull
{
     public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
     {
          if(request is not ICacheableQuery cacheableQuery)
          {
               return await next(cancellationToken);
          }
          if(await cacheService.GetAsync<TResponse>(cacheableQuery.CacheKey, cancellationToken) is TResponse cachedResponse)
          {
               logger.LogInformation("{RequestName} Cache hit for key {CacheKey}", typeof(TRequest).Name, cacheableQuery.CacheKey);
               return cachedResponse!;
          }
          logger.LogInformation("Cache miss for {RequestName}. CacheKey: {CacheKey}", typeof(TRequest).Name, cacheableQuery.CacheKey);
          
          var response = await next(cancellationToken);
          await cacheService.SetAsync(cacheableQuery.CacheKey, response, cacheableQuery.Expiration, cancellationToken);
          logger.LogInformation("{RequestName} Cache set for key {CacheKey} {Expiration}", typeof(TRequest).Name,
               cacheableQuery.CacheKey, cacheableQuery.Expiration);
          return response;
     }
}