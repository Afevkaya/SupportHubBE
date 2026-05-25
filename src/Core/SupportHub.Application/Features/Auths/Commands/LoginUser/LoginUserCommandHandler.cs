using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Domain.Entities.Identity;

namespace SupportHub.Application.Features.Auths.Commands.LoginUser;

public class LoginUserCommandHandler(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
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
        var responseCreateToken = tokenService.GenerateToken(user.Id, user.Email!, $"{user.FirstName} {user.LastName}",
            userRoles, cancellationToken: cancellationToken);
        
        logger.LogInformation("User with email: {Email} logged in successfully.", request.Email);
        return new LoginUserCommandResponse(
            responseCreateToken.AccessToken, 
            responseCreateToken.ExpireAt, 
            user.Id,
            user.Email, 
            $"{user.FirstName} {user.LastName}");

    }
}