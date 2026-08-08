using Dapper;
using SplitSpace.Auth.Dal.Database.Connections;
using SplitSpace.Auth.Dal.Database.Entities;
using SplitSpace.Auth.Domain.Models.Aggregates.RefreshTokenAggregate;
using SplitSpace.Auth.Domain.Models.Ids;

namespace SplitSpace.IntegrationTests.Helpers.Auth.TestRepositories;

public sealed class RefreshTokenTestRepository
{
    private readonly IRefreshTokenDomainRepository _domainRepository;
    private readonly IAuthDbConnectionFactory _connectionFactory;

    public RefreshTokenTestRepository(
        IRefreshTokenDomainRepository domainRepository,
        IAuthDbConnectionFactory connectionFactory)
    {
        _domainRepository = domainRepository;
        _connectionFactory = connectionFactory;
    }

    public Task SaveAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        return _domainRepository.SaveAsync(refreshToken, ct);
    }

    public Task SaveAsync(IReadOnlyCollection<RefreshToken> refreshTokens, CancellationToken ct = default)
    {
        return _domainRepository.SaveAsync(refreshTokens, ct);
    }

    public Task<RefreshToken?> GetAsync(string refreshToken, CancellationToken ct = default)
    {
        return _domainRepository.GetAsync(refreshToken, ct);
    }

    public async Task<RefreshTokenEntity?> ReadAsync(RefreshTokenId refreshTokenId, CancellationToken ct = default)
    {
        const string sql =
            """
            SELECT
                id,
                user_id AS UserId,
                token,
                expires_at AS ExpiresAt,
                revoked_at AS RevokedAt,
                created_at AS CreatedAt
            FROM refresh_token
            WHERE id = @Id
            """;

        await using var connection = await _connectionFactory.CreateAsync(ct);
        var entity = (await connection.QueryAsync<RefreshTokenEntity>(
            sql,
            new { Id = refreshTokenId.Value })).SingleOrDefault();
        return entity;
    }
}
