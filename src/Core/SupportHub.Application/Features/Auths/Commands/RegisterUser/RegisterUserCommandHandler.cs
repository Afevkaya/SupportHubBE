using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportHub.Domain.Entities.Identity;

namespace SupportHub.Application.Features.Auths.Commands.RegisterUser;

public class RegisterUserCommandHandler(
    UserManager<AppUser> userManager,
    ILogger<RegisterUserCommandHandler> logger) : IRequestHandler<RegisterUserCommand, RegisterUserCommandResponse>
{
    public async Task<RegisterUserCommandResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim() ?? string.Empty;

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            logger.LogWarning("Attempt to register with already registered email: {Email}", email);
            throw new ArgumentException("Email already registered.");
        }
        
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = email,
            UserName = email,
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogWarning("Failed to register user with email {Email}. Errors: {Errors}", email, errors);
            throw new InvalidOperationException($"Failed to register user: {errors}");
        }
        logger.LogInformation("User registered. UserId: {UserId}, Email: {Email}", user.Id, user.Email);
        return new RegisterUserCommandResponse(user.Id, user.Email, $"{user.FirstName} {user.LastName}");
    }
}