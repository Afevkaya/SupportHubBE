using System.Diagnostics;

namespace SupportHub.Api.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("HTTP {Method} {Path} started", context.Request.Method, context.Request.Path);
        await next(context);
        stopwatch.Stop();
        logger.LogInformation("HTTP {Method} {Path} {StatusCode} in {ElapsedMilliseconds}ms", context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}