using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Auths.Commands.Logout;

public record LogoutCommand(string RefreshToken) : ICommand<LogoutCommandResponse>;