using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.DTOs.Responses.Tokens;
using SupportHub.Domain.Entities.Identity;

namespace SupportHub.Application.Features.Auths.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    ITokenService tokenService,
    IRefreshTokenRepository refreshTokenRepository,
    UserManager<AppUser> userManager,
    ILogger<RefreshTokenCommandHandler> logger) : IRequestHandler<RefreshTokenCommand, RefreshTokenCommandResponse>
{
    public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var claimsPrincipal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if(claimsPrincipal == null)
        {
            logger.LogWarning("Invalid refresh token attempt. Reason: {Reason}", "Invalid refresh token.");
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }
        
        var userIdValue = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(!Guid.TryParse(userIdValue, out var userId))
        {
            logger.LogWarning("Invalid refresh token attempt. Reason: {Reason}", "Invalid refresh token.");
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }
        
        var refreshTokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        
        var storedRefreshToken = await refreshTokenRepository.GetByTokenHashAsync(refreshTokenHash);
        
        if (storedRefreshToken is null || storedRefreshToken.UserId != userId || storedRefreshToken.ExpiresAt <= now)
        {
            logger.LogWarning("Invalid refresh token attempt. Reason: {Reason}", "Invalid refresh token.");
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        if (storedRefreshToken.RevokedAt != null || storedRefreshToken.ReplacedByTokenHash != null)
        {
            logger.LogWarning(
                "Possible refresh token reuse detected. UserId: {UserId}, RefreshTokenId: {RefreshTokenId}, RevokedAt: {RevokedAt}, ReplacedByTokenHash exists: {ReplacedByTokenHashExists}",
                storedRefreshToken.UserId,
                storedRefreshToken.Id,
                storedRefreshToken.RevokedAt,
                storedRefreshToken.ReplacedByTokenHash is not null);

            throw new UnauthorizedAccessException("Invalid refresh token.");
        }
        
        var user = await userManager.FindByIdAsync(userId.ToString());
        if(user == null)
        {
            logger.LogWarning("Invalid refresh token attempt. Reason: {Reason}", "Invalid refresh token.");
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        ResponseCreateToken? newAccessToken = null;
        if (user.Email != null)
        {
            newAccessToken = tokenService.GenerateToken(user.Id, user.Email,
                $"{user.FirstName} {user.LastName}", await userManager.GetRolesAsync(user), cancellationToken);
        }
        if(newAccessToken is  null)
            throw new InvalidOperationException("Failed to generate new access token.");
        
        var newRefreshToken = tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = tokenService.HashRefreshToken(newRefreshToken);
        
        storedRefreshToken.RevokedAt = now;
        storedRefreshToken.ReasonRevoked = "Token rotated";
        storedRefreshToken.ReplacedByTokenHash = newRefreshTokenHash;
        
        var refreshToken = new Domain.Entities.Identity.RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshTokenHash,
            ExpiresAt = now.AddDays(7)
        };
        
        await refreshTokenRepository.AddAsync(refreshToken);
        logger.LogInformation(
            "Refresh token rotated successfully. UserId: {UserId}, RefreshTokenId: {RefreshTokenId}",
            user.Id,
            storedRefreshToken.Id);
        
        return new RefreshTokenCommandResponse(newAccessToken.AccessToken, newRefreshToken, newAccessToken.ExpirationAt);
    }
}
