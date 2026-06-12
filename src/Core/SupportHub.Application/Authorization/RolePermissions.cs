using SupportHub.Application.Constants;

namespace SupportHub.Application.Authorization;

public static class RolePermissions
{
    public static readonly IReadOnlyDictionary<string, IReadOnlyCollection<string>> PermissionsByRole =
        new Dictionary<string, IReadOnlyCollection<string>>
        {
            [Roles.Customer] =
            [
                Permissions.Tickets.View,
                Permissions.Tickets.Create,
                Permissions.Tickets.Comment
            ],

            [Roles.SupportAgent] =
            [
                Permissions.Tickets.View,
                Permissions.Tickets.Update,
                Permissions.Tickets.Assign,
                Permissions.Tickets.Comment,
                Permissions.Tickets.Close
            ],

            [Roles.Admin] = Permissions.All
        };
}