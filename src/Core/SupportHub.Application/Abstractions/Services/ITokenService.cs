using SupportHub.Application.DTOs.Responses.Tokens;

namespace SupportHub.Application.Abstractions.Services;

public interface ITokenService
{
    ResponseCreateToken GenerateToken(Guid userId, string email, string fullName, IEnumerable<string>? roles = null,
        CancellationToken cancellationToken = default);
}