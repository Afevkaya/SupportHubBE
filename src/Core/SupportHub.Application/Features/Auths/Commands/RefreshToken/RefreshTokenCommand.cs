using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Auths.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<RefreshTokenCommandResponse>;