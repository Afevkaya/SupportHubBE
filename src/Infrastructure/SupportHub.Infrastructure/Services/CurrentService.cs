using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SupportHub.Application.Abstractions.Services;

namespace SupportHub.Infrastructure.Services;

public class CurrentService(IHttpContextAccessor httpContextAccessor) : ICurrentService
{
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public string? Email => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    public Guid? UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            return Guid.TryParse(value, out var userId)
                ? userId
                : null;
        }
    }

    public string? FullName => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value + " " +
                               httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value;
}
