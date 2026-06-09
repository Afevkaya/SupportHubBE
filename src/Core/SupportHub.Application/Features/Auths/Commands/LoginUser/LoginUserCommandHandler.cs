using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Domain.Entities.Identity;

namespace SupportHub.Application.Features.Auths.Commands.LoginUser;

public class LoginUserCommandHandler(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IRefreshTokenRepository  refreshTokenRepository,
    ILogger<LoginUserCommandHandler> logger) : IRequestHandler<LoginUserCommand, LoginUserCommandResponse>
{
    public async Task<LoginUserCommandResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            logger.LogWarning("Login attempt failed for email: {Email}. User not found.", request.Email);
            throw new UnauthorizedAccessException("Email veya şifre hatalı.");
        }
        
        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            logger.LogWarning("Login attempt failed for email: {Email}. Invalid password.", request.Email);
            throw new UnauthorizedAccessException("Email veya şifre hatalı.");
        }

        if (user.Email is null)
        {
            logger.LogWarning("Login attempt failed for email: {Email}. Email not found.", request.Email);
            throw new Exception("Kullanıcının email adresi bulunamadı.");
        }
        var userRoles = await userManager.GetRolesAsync(user);
        
        var accessToken = tokenService.GenerateToken(user.Id, user.Email!, $"{user.FirstName} {user.LastName}",
            userRoles, cancellationToken: cancellationToken);
        var refreshToken = tokenService.GenerateRefreshToken();
        var refreshTokeHash = tokenService.HashRefreshToken(refreshToken);
        
        var refreshTokenEntity = new Domain.Entities.Identity.RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokeHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = null
        };
        
        await refreshTokenRepository.AddAsync(refreshTokenEntity);
        
        logger.LogInformation("User with email: {Email} and userid {UserId} logged in successfully.", request.Email, user.Id);
        return new LoginUserCommandResponse(
            accessToken.AccessToken, 
            refreshToken,
            accessToken.ExpireAt, 
            user.Id,
            user.Email, 
            $"{user.FirstName} {user.LastName}");

    }
}