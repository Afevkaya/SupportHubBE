using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Auths.Commands.LoginUser;

public record LoginUserCommand(string Email, string Password) : ICommand<LoginUserCommandResponse>;