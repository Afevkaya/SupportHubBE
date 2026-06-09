namespace SupportHub.Application.Features.Auths.Commands.RefreshToken;

public record RefreshTokenCommandResponse(string AccessToken, string RefreshToken, DateTime ExpirationAt);