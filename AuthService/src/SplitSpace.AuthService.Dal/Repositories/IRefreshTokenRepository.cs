using SplitSpace.AuthService.Dal.Models.Entities;

namespace SplitSpace.AuthService.Dal.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task CreateRefreshTokenAsync(RefreshToken token);
    Task RevokeRefreshTokenAsync(Guid tokenId);
    Task<List<RefreshToken>> GetAllUserTokensAsync(Guid userId);
}
