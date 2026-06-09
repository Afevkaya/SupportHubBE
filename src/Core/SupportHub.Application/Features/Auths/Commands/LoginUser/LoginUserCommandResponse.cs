namespace SupportHub.Application.Features.Auths.Commands.LoginUser;

public record LoginUserCommandResponse(
    string AccessToken, 
    string RefreshToken,
    DateTime ExpirationAt, 
    Guid UserId, 
    string Email, 
    string FullName);