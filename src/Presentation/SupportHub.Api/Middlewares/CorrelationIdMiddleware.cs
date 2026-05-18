using Serilog.Context;

namespace SupportHub.Api.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-Correlation-Id", out var correlationId) || string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        var correlationIdValue = correlationId.ToString();

        context.Items["CorrelationId"] = correlationIdValue;
        context.Response.Headers["X-Correlation-Id"] = correlationIdValue;

        using (LogContext.PushProperty("CorrelationId", correlationIdValue))
        {
            await next(context);
        }
    }
}