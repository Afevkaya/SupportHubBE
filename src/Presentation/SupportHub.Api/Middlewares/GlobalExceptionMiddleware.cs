using System.Net;
using System.Text.Json;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Api.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            context.Response.ContentType = "application/json";

            var statusCode = e switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                ArgumentNullException or ArgumentException or FormatException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            // If exception has a public int StatusCode property, prefer it.
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
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var payload = JsonSerializer.Serialize(error, options);
            await context.Response.WriteAsync(payload);
        }
    }
}