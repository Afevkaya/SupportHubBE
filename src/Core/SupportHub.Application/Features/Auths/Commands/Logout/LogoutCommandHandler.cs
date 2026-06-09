using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories;
using SupportHub.Application.Abstractions.Services;

namespace SupportHub.Application.Features.Auths.Commands.Logout;

public class LogoutCommandHandler(
    ITokenService tokenService,
    IRefreshTokenRepository refreshTokenRepository,
    ILogger<LogoutCommandHandler> logger) : IRequestHandler<LogoutCommand, LogoutCommandResponse>
{
    public async Task<LogoutCommandResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        var isExistRefreshTokenHash = await refreshTokenRepository.GetByTokenHashAsync(refreshTokenHash);

        if (isExistRefreshTokenHash is null)
        {
            logger.LogWarning("Invalid refresh token attempt. Reason: {Reason}", "Invalid refresh token.");
            return new LogoutCommandResponse(true, "Logout successful");
        }
        
        if(isExistRefreshTokenHash.RevokedAt is not null || isExistRefreshTokenHash.ReplacedByTokenHash is not null)
        {
            logger.LogWarning(
                "Possible refresh token reuse detected. UserId: {UserId}, RefreshTokenId: {RefreshTokenId}, RevokedAt: {RevokedAt}, ReplacedByTokenHash exists: {ReplacedByTokenHashExists}",
                isExistRefreshTokenHash.UserId,
                isExistRefreshTokenHash.Id,
                isExistRefreshTokenHash.RevokedAt,
                isExistRefreshTokenHash.ReplacedByTokenHash is not null);

            return new LogoutCommandResponse(true, "Logout successful");
        }
        
        isExistRefreshTokenHash.RevokedAt = DateTime.UtcNow;
        logger.LogInformation(
            "User logged out successfully. UserId: {UserId}, RefreshTokenId: {RefreshTokenId}",
            isExistRefreshTokenHash.UserId,
            isExistRefreshTokenHash.Id);
        
        return new LogoutCommandResponse(true, "Logout successful");
    }
}
