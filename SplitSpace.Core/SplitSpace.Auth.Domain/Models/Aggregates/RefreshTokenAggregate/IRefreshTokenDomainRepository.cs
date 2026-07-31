namespace SplitSpace.Auth.Domain.Models.Aggregates.RefreshTokenAggregate;

public interface IRefreshTokenDomainRepository
{
    Task SaveAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    
    Task SaveAsync(IReadOnlyCollection<RefreshToken> refreshTokens, CancellationToken cancellationToken);
    
    Task<RefreshToken?> GetAsync(string refreshToken, CancellationToken cancellationToken);
}
