using Microsoft.Extensions.DependencyInjection;
using SupportHub.Application.Abstractions.Caching;
using SupportHub.Infrastructure.Caching;

namespace SupportHub.Infrastructure.Extensions;

public static class ServiceRegistrationExtension
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ICacheService,MemoryCacheService>();
    }
}