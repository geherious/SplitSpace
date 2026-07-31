using System.Data;
using Dapper;
using Npgsql;
using SplitSpace.Auth.Dal.Database.Connections;
using SplitSpace.Auth.Dal.Database.Entities;
using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;
using SplitSpace.Auth.Domain.Models.Events;
using SplitSpace.Auth.Domain.Models.Ids;
using SplitSpace.Auth.Domain.Models.ValueObjects;

namespace SplitSpace.Auth.Dal.Database.Repositories.Implementations;

public class UserDomainRepository : BaseRepository, IUserDomainRepository
{
    public UserDomainRepository(IAuthDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public UserDomainRepository(IAuthDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
        : base(connectionFactory, transaction)
    {
    }

    public async Task<User?> GetAsync(UserId userId, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);

        var entity = await connection.QuerySingleOrDefaultAsync<UserEntity>(
            """
            SELECT
                id,
                email,
                password_hash AS PasswordHash,
                created_at AS CreatedAt,
                last_login AS LastLogin
            FROM "user"
            WHERE id = @Id
            """,
            new { Id = userId.Value });

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<User?> GetAsync(Email email, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);

        var entity = await connection.QuerySingleOrDefaultAsync<UserEntity>(
            """
            SELECT
                id,
                email,
                password_hash AS PasswordHash,
                created_at AS CreatedAt,
                last_login AS LastLogin
            FROM "user"
            WHERE email = @Email
            """,
            new { Email = email.Value });

        return entity is null ? null : ToDomain(entity);
    }

    public async Task SaveAsync(User user, CancellationToken cancellationToken)
    {
        if (_transaction is not null)
        {
            await ProcessEventsAsync(user, _transaction);
            user.ClearDomainEvents();
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);
        await using var tx = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            await ProcessEventsAsync(user, tx);
            user.ClearDomainEvents();
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task ProcessEventsAsync(User user, IDbTransaction transaction)
    {
        var connection = transaction.Connection!;

        foreach (var domainEvent in user.DomainEvents)
        {
            switch (domainEvent)
            {
                case UserCreatedEvent e:
                    await connection.ExecuteAsync(
                        """
                        INSERT INTO "user" (id, email, password_hash, created_at, last_login)
                        VALUES (@Id, @Email, @PasswordHash, @CreatedAt, @LastLogin)
                        """,
                        new
                        {
                            Id = e.User.Id.Value,
                            Email = e.User.Email.Value,
                            PasswordHash = e.User.PasswordHash.Value,
                            CreatedAt = e.User.CreatedAt,
                            LastLogin = e.User.LastLogin
                        },
                        transaction);
                    break;
            }
        }
    }

    private static User ToDomain(UserEntity entity)
        => User.Rehydrate(
            new UserId(entity.Id),
            new Email(entity.Email),
            new PasswordHash(entity.PasswordHash),
            entity.CreatedAt,
            entity.LastLogin);
}
