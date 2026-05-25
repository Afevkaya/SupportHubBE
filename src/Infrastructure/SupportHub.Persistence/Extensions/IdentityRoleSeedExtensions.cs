using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Persistence.Extensions;

public static class IdentityRoleSeedExtensions
{
    private static readonly string[] DefaultRoles = ["Admin", "SupportAgent", "Customer"];

    public static async Task SeedDefaultRolesAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("IdentityRoleSeeder");

        foreach (var roleName in DefaultRoles)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Role seed failed for '{roleName}'. Errors: {errors}");
            }

            logger.LogInformation("Default role seeded: {RoleName}", roleName);
        }
    }
}
