using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Events;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class BalanceDomainRepository : IBalanceDomainRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public BalanceDomainRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal BalanceDomainRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task SaveAsync(Balance balance, CancellationToken cancellationToken)
    {
        if (_transaction is not null)
        {
            await ApplyEventsAsync(_transaction, balance, cancellationToken);
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            await ApplyEventsAsync(transaction, balance, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task ApplyEventsAsync(NpgsqlTransaction transaction, Balance balance, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in balance.DomainEvents)
        {
            switch (domainEvent)
            {
                case BalanceCreatedEvent e:
                    await OnBalanceCreatedAsync(transaction, e);
                    break;
                case BalanceAmountChangedEvent e:
                    await OnBalanceAmountChangedAsync(transaction, e);
                    break;
            }
        }
        
        balance.ClearDomainEvents();
    }

    private static async Task OnBalanceCreatedAsync(NpgsqlTransaction transaction, BalanceCreatedEvent e)
    {
        const string sql =
            """
            INSERT INTO balance (id, name, total, owner_type, owner_id, created_by)
            VALUES (@Id, @Name, @Total, @OwnerType, @OwnerId, @CreatedBy)
            """;

        var parameters = new
        {
            Id = e.Balance.Id.Value,
            e.Balance.Name,
            Total = e.Balance.Total.Amount,
            OwnerType = e.Balance.OwnerType.ToString(),
            e.Balance.OwnerId,
            e.Balance.CreatedBy
        };

        await transaction.Connection!.ExecuteAsync(sql, parameters, transaction);
    }

    private static async Task OnBalanceAmountChangedAsync(NpgsqlTransaction transaction, BalanceAmountChangedEvent e)
    {
        const string updateSql =
            """
            UPDATE balance
            SET total = @Total
            WHERE id = @Id
            """;

        const string insertEntrySql =
            """
            INSERT INTO balance_entry (id, balance_id, total, source_type, expense_id)
            VALUES (@Id, @BalanceId, @Total, @SourceType, @ExpenseId)
            ON CONFLICT (balance_id, source_type, expense_id) DO NOTHING
            """;

        var connection = transaction.Connection!;

        await connection.ExecuteAsync(
            updateSql,
            new { Id = e.Balance.Id.Value, Total = e.Balance.Total.Amount },
            transaction);

        var expenseSource = e.Entry.Source as BalanceEntry.BalanceEntrySource.Expense;
        var entryParameters = new
        {
            Id = e.Entry.Id.Value,
            BalanceId = e.Entry.BalanceId.Value,
            Total = e.Entry.Total.Amount,
            SourceType = expenseSource is null ? "none" : "expense",
            ExpenseId = expenseSource?.ExpenseId.Value
        };

        await connection.ExecuteAsync(insertEntrySql, entryParameters, transaction);
    }

    public async Task<Balance?> GetAsync(BalanceId balanceId, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT
                id,
                name,
                total,
                owner_type AS OwnerType,
                owner_id AS OwnerId,
                created_by AS CreatedBy
            FROM balance
            WHERE id = @Id
            """;

        if (_transaction is not null)
        {
            var entity = await _transaction.Connection!.QuerySingleOrDefaultAsync<BalanceEntity>(
                sql, new { Id = balanceId.Value }, _transaction);
            return entity is null ? null : Map(entity);
        }

        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);
        var result = await connection.QuerySingleOrDefaultAsync<BalanceEntity>(sql, new { Id = balanceId.Value });
        return result is null ? null : Map(result);
    }

    private static Balance Map(BalanceEntity entity)
    {
        return Balance.Rehydrate(
            new BalanceId(entity.Id),
            entity.Name,
            new Money(entity.Total),
            entity.OwnerType,
            entity.OwnerId,
            entity.CreatedBy);
    }
}
