using Microsoft.AspNetCore.Identity;

namespace SupportHub.Domain.Entities.Identity;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}