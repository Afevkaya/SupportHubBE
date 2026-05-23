using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Auths.Commands.RegisterUser;

public record RegisterUserCommand(string FirstName, string LastName, string Email, string Password) : ICommand<RegisterUserCommandResponse>;