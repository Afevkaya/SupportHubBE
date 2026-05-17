using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SupportHub.Application.Abstractions.Services;

namespace SupportHub.Application.Extensions;

public static class ServiceRegistrationExtension
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}