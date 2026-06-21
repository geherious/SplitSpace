using System.Data;
using Dapper;
using Npgsql;
using SplitSpace.SpaceService.Dal.Database.Connections;
using SplitSpace.SpaceService.Dal.Database.Entities;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;
using SplitSpace.SpaceService.Domain.Models.Events;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Dal.Database.Repositories.Implementations;

public class SpaceDomainRepository : ISpaceDomainRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public SpaceDomainRepository(IDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal SpaceDomainRepository(IDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task<Space?> GetAsync(SpaceId spaceId, CancellationToken ct)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);

        var entity = await connection.QuerySingleOrDefaultAsync<SpaceEntity>(
            """
            SELECT
                id,
                name,
                type,
                owner_id AS OwnerId,
                created_at AS CreatedAt
            FROM space
            WHERE id = @Id
            """,
            new { Id = spaceId.Value });

        if (entity is null)
            return null;

        var memberEntities = (await connection.QueryAsync<SpaceMemberEntity>(
            """
            SELECT
                id,
                space_id AS SpaceId,
                user_id AS UserId,
                role,
                joined_at AS JoinedAt
            FROM space_membership
            WHERE space_id = @SpaceId
            """,
            new { SpaceId = spaceId.Value })).ToList();

        var members = memberEntities
            .Select(m => new SpaceMember(
                new SpaceMemberId(m.Id),
                new UserId(m.UserId),
                new SpaceId(m.SpaceId),
                Enum.Parse<SpaceMemberRole>(m.Role),
                m.JoinedAt))
            .ToList();

        return new Space(
            new SpaceId(entity.Id),
            entity.Name,
            Enum.Parse<SpaceType>(entity.Type),
            new UserId(entity.OwnerId),
            members,
            entity.CreatedAt);
    }

    public async Task SaveAsync(Space space, CancellationToken ct = default)
    {
        if (_transaction is not null)
        {
            await ProcessEventsAsync(space, _transaction);
            space.ClearDomainEvents();
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var tx = await connection.BeginTransactionAsync(ct);
        try
        {
            await ProcessEventsAsync(space, tx);
            space.ClearDomainEvents();
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private static async Task ProcessEventsAsync(Space space, IDbTransaction transaction)
    {
        var connection = transaction.Connection!;

        foreach (var domainEvent in space.DomainEvents)
        {
            switch (domainEvent)
            {
                case SpaceCreatedEvent e:
                    await connection.ExecuteAsync(
                        """
                        INSERT INTO space (id, name, type, owner_id, created_at)
                        VALUES (@Id, @Name, @Type, @OwnerId, @CreatedAt)
                        """,
                        new { Id = e.Id.Value, e.Name, Type = e.Type.ToString(), OwnerId = e.OwnerId.Value, e.CreatedAt },
                        transaction);
                    break;

                case SpaceMemberAddedEvent e:
                    await connection.ExecuteAsync(
                        """
                        INSERT INTO space_membership (id, space_id, user_id, role, joined_at)
                        VALUES (@Id, @SpaceId, @UserId, @Role, @JoinedAt)
                        """,
                        new
                        {
                            Id = e.MemberId.Value,
                            SpaceId = e.SpaceId.Value,
                            UserId = e.UserId.Value,
                            Role = e.Role.ToString(),
                            e.JoinedAt
                        },
                        transaction);
                    break;

                case SpaceDeletedEvent e:
                    await connection.ExecuteAsync(
                        "DELETE FROM space WHERE id = @Id",
                        new { Id = e.Id.Value },
                        transaction);
                    break;
            }
        }
    }
}
