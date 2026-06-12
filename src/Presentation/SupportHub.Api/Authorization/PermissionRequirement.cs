using Microsoft.AspNetCore.Authorization;

namespace SupportHub.Api.Authorization;

public sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;