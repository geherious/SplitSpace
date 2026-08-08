using System.Data;
using Dapper;
using Npgsql;
using SplitSpace.Spaces.Dal.Database.Connections;
using SplitSpace.Spaces.Dal.Database.Entities;
using SplitSpace.Spaces.Domain.Models.Aggregates.InvitationAggregate;
using SplitSpace.Spaces.Domain.Models.Events;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Dal.Database.Repositories.Implementations;

public class InvitationDomainRepository : IInvitationDomainRepository
{
    private readonly ISpaceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public InvitationDomainRepository(ISpaceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal InvitationDomainRepository(ISpaceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task<Invitation?> GetAsync(InvitationId invitationId, CancellationToken ct)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);

        var entity = await connection.QuerySingleOrDefaultAsync<InvitationEntity>(
            """
            SELECT
                id,
                space_id AS SpaceId,
                invited_user_id AS InvitedUserId,
                invited_by AS InvitedBy,
                status,
                expires_at AS ExpiresAt,
                created_at AS CreatedAt
            FROM invitation
            WHERE id = @Id
            """,
            new { Id = invitationId.Value });

        if (entity is null)
            return null;

        return Invitation.Rehydrate(
            new InvitationId(entity.Id),
            new SpaceId(entity.SpaceId),
            new UserId(entity.InvitedUserId),
            new UserId(entity.InvitedBy),
            Enum.Parse<InvitationStatus>(entity.Status),
            entity.ExpiresAt,
            entity.CreatedAt);
    }

    public async Task SaveAsync(Invitation invitation, CancellationToken ct = default)
    {
        if (_transaction is not null)
        {
            await ProcessEventsAsync(invitation, _transaction);
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var tx = await connection.BeginTransactionAsync(ct);
        try
        {
            await ProcessEventsAsync(invitation, tx);
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private static async Task ProcessEventsAsync(Invitation invitation, IDbTransaction transaction)
    {
        var connection = transaction.Connection!;

        foreach (var domainEvent in invitation.DomainEvents)
        {
            switch (domainEvent)
            {
                case InvitationCreatedEvent e:
                    await connection.ExecuteAsync(
                        """
                        INSERT INTO invitation (id, space_id, invited_user_id, invited_by, status, expires_at, created_at)
                        VALUES (@Id, @SpaceId, @InvitedUserId, @InvitedBy, @Status, @ExpiresAt, @CreatedAt)
                        """,
                        new
                        {
                            Id = e.Id.Value,
                            SpaceId = e.SpaceId.Value,
                            InvitedUserId = e.InvitedUserId.Value,
                            InvitedBy = e.InvitedBy.Value,
                            Status = "Created",
                            e.ExpiresAt,
                            e.CreatedAt
                        },
                        transaction);
                    break;

                case InvitationAcceptedEvent e:
                    await connection.ExecuteAsync(
                        "UPDATE invitation SET status = @Status WHERE id = @Id",
                        new { Id = e.Id.Value, Status = "Accepted" },
                        transaction);
                    break;

                case InvitationRejectedEvent e:
                    await connection.ExecuteAsync(
                        "UPDATE invitation SET status = @Status WHERE id = @Id",
                        new { Id = e.Id.Value, Status = "Rejected" },
                        transaction);
                    break;
            }
        }

        invitation.ClearDomainEvents();
    }
}
