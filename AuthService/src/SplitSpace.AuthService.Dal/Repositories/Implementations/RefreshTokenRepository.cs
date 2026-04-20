using Microsoft.EntityFrameworkCore;
using SplitSpace.AuthService.Dal.Models.Entities;
using SplitSpace.AuthService.Dal.Repositories;

namespace SplitSpace.AuthService.Dal.repositories.Implementations;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthServiceDbContext _context;

    public RefreshTokenRepository(AuthServiceDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task CreateRefreshTokenAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeRefreshTokenAsync(Guid tokenId)
    {
        var token = await GetByTokenIdAsync(tokenId);
        if (token != null)
        {
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<RefreshToken>> GetAllUserTokensAsync(Guid userId)
    {
        return await _context.RefreshTokens
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    private async Task<RefreshToken?> GetByTokenIdAsync(Guid tokenId)
    {
        return await _context.RefreshTokens.FindAsync(tokenId);
    }
}
