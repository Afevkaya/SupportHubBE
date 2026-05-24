using System.Net;
using System.Text.Json;
using FluentValidation;
using SupportHub.Api.Models.Responses;

namespace SupportHub.Api.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            context.Response.ContentType = "application/json";
            
            if (e is ValidationException vex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var errors = vex.Errors
                    .Where(x => !string.IsNullOrWhiteSpace(x.PropertyName))
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(f => f.ErrorMessage ?? string.Empty).ToList()
                    );

                var errorResponse = new ValidationErrorResponse(
                    context.Response.StatusCode,
                    "Validation failed.",
                    errors
                );
                
                var payload = JsonSerializer.Serialize(errorResponse, JsonOptions);
                await context.Response.WriteAsync(payload);
                logger.LogError(vex, "Validation failed.");
                return;
            }

            var statusCode = e switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                ArgumentNullException or ArgumentException or FormatException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                ValidationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var statusProp = e.GetType().GetProperty("StatusCode");
            if (statusProp != null && statusProp.PropertyType == typeof(int))
            {
                try
                {
                    var val = statusProp.GetValue(e);
                    if (val is int v and >= 100 and <= 599)
                        statusCode = v;
                    
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }

            context.Response.StatusCode = statusCode;
            var error = new ErrorResponse(statusCode, e.Message);
            var payload2 = JsonSerializer.Serialize(error, JsonOptions);
            await context.Response.WriteAsync(payload2);
            logger.LogError(e, "An unexpected error occurred.");
        }
    }
}