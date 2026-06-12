using Microsoft.AspNetCore.Authorization;
using SupportHub.Api.Authorization;
using SupportHub.Application.Authorization;

namespace SupportHub.Api.Extensions;

public static class ServiceRegistrationExtension
{
    public static void AddPresentationServices(this IServiceCollection services)
    {
        AddPermissionPolicies(services);
    }
    
    private static void AddPermissionPolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            foreach (var permission in Permissions.All)
            {
                options.AddPolicy(permission, policy => policy.AddRequirements(new PermissionRequirement(permission)));
            }
        });
        
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
    }
}