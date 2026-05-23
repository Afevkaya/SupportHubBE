namespace SupportHub.Application.Features.Auths.Commands.RegisterUser;

public record RegisterUserCommandResponse(Guid UserId, string Email, string FullName);