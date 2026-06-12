namespace SupportHub.Application.Authorization;

public static class Permissions
{
    public static class Tickets
    {
        public const string View = "Tickets.View";
        public const string Create = "Tickets.Create";
        public const string Update = "Tickets.Update";
        public const string Comment = "Tickets.Comment";
        public const string Assign = "Tickets.Assign";
        public const string Close = "Tickets.Close";
    }
    
    public static class Users
    {
        public const string View = "Users.View";
        public const string Manage = "Users.Manage";
    }
    
    public static class Roles
    {
        public const string View = "Roles.View";
        public const string Manage = "Roles.Manage";
    }
    
    public static readonly IReadOnlyCollection<string> All =
    [
        Tickets.View,
        Tickets.Create,
        Tickets.Update,
        Tickets.Assign,
        Tickets.Comment,
        Tickets.Close,
        Users.View,
        Users.Manage,
        Roles.View,
        Roles.Manage
    ];
}