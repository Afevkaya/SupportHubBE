using Microsoft.Extensions.DependencyInjection;
using SupportHub.Application.Abstractions.Caching;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Infrastructure.Caching;
using SupportHub.Infrastructure.Services;

namespace SupportHub.Infrastructure.Extensions;

public static class ServiceRegistrationExtension
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ICacheService, MemoryCacheService>();
        services.AddScoped<ITokenService, TokenService>();
    }
}
