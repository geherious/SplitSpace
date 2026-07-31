using System.Data;
using Dapper;
using Npgsql;
using SplitSpace.Auth.Dal.Database.Connections;
using SplitSpace.Auth.Dal.Database.Entities;
using SplitSpace.Auth.Domain.Models.Aggregates.RefreshTokenAggregate;
using SplitSpace.Auth.Domain.Models.Events;
using SplitSpace.Auth.Domain.Models.Ids;
using SplitSpace.Auth.Domain.Models.ValueObjects;

namespace SplitSpace.Auth.Dal.Database.Repositories.Implementations;

public class RefreshTokenDomainRepository : BaseRepository, IRefreshTokenDomainRepository
{
    public RefreshTokenDomainRepository(IAuthDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public RefreshTokenDomainRepository(IAuthDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
        : base(connectionFactory, transaction)
    {
    }

    public async Task SaveAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        => await SaveAsync([refreshToken], cancellationToken);

    public async Task SaveAsync(IReadOnlyCollection<RefreshToken> refreshTokens, CancellationToken cancellationToken)
    {
        if (_transaction is not null)
        {
            foreach (var refreshToken in refreshTokens)
            {
                await ProcessEventsAsync(refreshToken, _transaction);
                refreshToken.ClearDomainEvents();
            }
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);
        await using var tx = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var refreshToken in refreshTokens)
            {
                await ProcessEventsAsync(refreshToken, tx);
                refreshToken.ClearDomainEvents();
            }
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<RefreshToken?> GetAsync(string refreshToken, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);

        var entity = await connection.QuerySingleOrDefaultAsync<RefreshTokenEntity>(
            """
            SELECT
                id,
                user_id AS UserId,
                token,
                expires_at AS ExpiresAt,
                revoked_at AS RevokedAt,
                created_at AS CreatedAt
            FROM refresh_token
            WHERE token = @Token
              AND revoked_at IS NULL
            """,
            new { Token = RefreshTokenHash.FromRaw(refreshToken).Value });

        if (entity is null)
        {
            return null;
        }

        return RefreshToken.Rehydrate(
            new RefreshTokenId(entity.Id),
            new UserId(entity.UserId),
            new RefreshTokenHash(entity.Token),
            entity.ExpiresAt,
            entity.RevokedAt,
            entity.CreatedAt);
    }

    private static async Task ProcessEventsAsync(RefreshToken refreshToken, IDbTransaction transaction)
    {
        var connection = transaction.Connection!;

        foreach (var domainEvent in refreshToken.DomainEvents)
        {
            switch (domainEvent)
            {
                case RefreshTokenCreatedEvent e:
                    await connection.ExecuteAsync(
                        """
                        INSERT INTO refresh_token (id, user_id, token, expires_at, revoked_at, created_at)
                        VALUES (@Id, @UserId, @Token, @ExpiresAt, @RevokedAt, @CreatedAt)
                        """,
                        new
                        {
                            Id = e.RefreshToken.Id.Value,
                            UserId = e.RefreshToken.UserId.Value,
                            Token = e.RefreshToken.Token.Value,
                            ExpiresAt = e.RefreshToken.ExpiresAt,
                            RevokedAt = e.RefreshToken.RevokedAt,
                            CreatedAt = e.RefreshToken.CreatedAt
                        },
                        transaction);
                    break;

                case RefreshTokenRevokedEvent e:
                    await connection.ExecuteAsync(
                        "UPDATE refresh_token SET revoked_at = @RevokedAt WHERE id = @Id",
                        new
                        {
                            Id = e.RefreshToken.Id.Value,
                            RevokedAt = e.RefreshToken.RevokedAt
                        },
                        transaction);
                    break;
            }
        }
    }
}
