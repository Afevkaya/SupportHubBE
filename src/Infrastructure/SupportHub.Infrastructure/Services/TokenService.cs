using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.DTOs.Responses.Tokens;

namespace SupportHub.Infrastructure.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public ResponseCreateToken GenerateToken(Guid userId, string email, string fullName,
        IEnumerable<string>? roles = null,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        var secretKey = configuration["Jwt:SecretKey"]
                        ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
        var issuer = configuration["Jwt:Issuer"]
                     ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        var audience = configuration["Jwt:Audience"]
                       ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

        if (!int.TryParse(configuration["Jwt:ExpirationInMinutes"], out var expirationInMinutes))
        {
            expirationInMinutes = 60;
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Name, fullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Name, fullName)
        };
        if (roles != null)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddMinutes(expirationInMinutes);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new ResponseCreateToken
        {
            AccessToken = accessToken,
            ExpireAt = expiration
        };
    }
}
