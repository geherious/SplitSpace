using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Domain.Models.Aggregates.SettlementAggregate;
using SplitSpace.Finances.Domain.Models.Events;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class SettlementDomainRepository : ISettlementDomainRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public SettlementDomainRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal SettlementDomainRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task SaveAsync(Settlement settlement, CancellationToken cancellationToken)
    {
        if (_transaction is not null)
        {
            await ApplyEventsAsync(_transaction, settlement);
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            await ApplyEventsAsync(transaction, settlement);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task ApplyEventsAsync(NpgsqlTransaction transaction, Settlement settlement)
    {
        foreach (var domainEvent in settlement.DomainEvents)
        {
            switch (domainEvent)
            {
                case SettlementCreatedEvent e:
                    await OnSettlementCreatedAsync(transaction, e);
                    break;
            }
        }
        
        settlement.ClearDomainEvents();
    }

    private static async Task OnSettlementCreatedAsync(NpgsqlTransaction transaction, SettlementCreatedEvent e)
    {
        const string sql =
            """
            INSERT INTO settlement (id, from_user_id, from_balance_id, to_user_id, amount, created_at)
            VALUES (@Id, @FromUserId, @FromBalanceId, @ToUserId, @Amount, @CreatedAt)
            """;

        var parameters = new
        {
            Id = e.Settlement.Id.Value,
            FromUserId = e.Settlement.FromUserId.Value,
            FromBalanceId = e.Settlement.FromBalanceId.Value,
            ToUserId = e.Settlement.ToUserId.Value,
            Amount = e.Settlement.Amount.Amount,
            CreatedAt = e.Settlement.CreatedAt
        };

        await transaction.Connection!.ExecuteAsync(sql, parameters, transaction);
    }
}
