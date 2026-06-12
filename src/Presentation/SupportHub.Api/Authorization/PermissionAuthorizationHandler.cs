using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SupportHub.Application.Authorization;

namespace SupportHub.Api.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if(context.User.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }
        
        var roles = context.User
            .FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct()
            .ToList();
        
        if(roles.Count == 0)
        {
            return Task.CompletedTask;
        }

        var hasPermission = roles.Any(role =>
            RolePermissions.PermissionsByRole.TryGetValue(role, out var permissions) &&
            permissions.Contains(requirement.Permission));
        
        if(hasPermission)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}