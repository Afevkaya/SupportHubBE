using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Behaviors;

public class QueryPerformanceBehavior<TRequest,TResponse>(ILogger<QueryPerformanceBehavior<TRequest,TResponse>> logger)
    : IPipelineBehavior<TRequest,TResponse> where TRequest : IQuery<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await next(cancellationToken);
        stopwatch.Stop();
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        if (elapsedMilliseconds > 500)
            logger.LogWarning("Long running query detected: {RequestType} took {ElapsedMilliseconds} ms", typeof(TRequest).Name, elapsedMilliseconds);
        else
            logger.LogInformation("Query {RequestType} executed in {ElapsedMilliseconds} ms", typeof(TRequest).Name, elapsedMilliseconds);
        
        return response;
    }
}