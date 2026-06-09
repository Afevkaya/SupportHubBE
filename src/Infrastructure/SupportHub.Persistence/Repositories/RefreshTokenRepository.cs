using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using SupportHub.Application.Abstractions.Repositories;
using SupportHub.Domain.Entities.Identity;

namespace Persistence.Repositories;

public class RefreshTokenRepository(SupportHubDbContext dbContext) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        return await dbContext.RefreshTokens
            .Include(r=>r.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await dbContext.RefreshTokens.AddAsync(refreshToken);
    }
}