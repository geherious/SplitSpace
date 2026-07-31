using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
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
        const string sql =
            """
            INSERT INTO balance (id, name, balance, owner_type, owner_id, created_by)
            VALUES (@Id, @Name, @Balance, @OwnerType, @OwnerId, @CreatedBy)
            ON CONFLICT (id) DO UPDATE SET
                balance = EXCLUDED.balance
            """;

        var parameters = new
        {
            Id = balance.Id.Value,
            balance.Name,
            Balance = balance.Total.Amount,
            balance.OwnerType,
            balance.OwnerId,
            balance.CreatedBy
        };

        if (_transaction is not null)
        {
            await _transaction.Connection!.ExecuteAsync(sql, parameters, _transaction);
        }
        else
        {
            await using var connection = await _connectionFactory.CreateAsync(cancellationToken);
            await connection.ExecuteAsync(sql, parameters);
        }

        if (balance.Entries.Count > 0)
        {
            await SaveEntriesAsync(balance, cancellationToken);
        }
    }

    private async Task SaveEntriesAsync(Balance balance, CancellationToken cancellationToken)
    {
        const string sql =
            """
            INSERT INTO balance_entry (id, balance_id, total, source_type, expense_id)
            VALUES (@Id, @BalanceId, @Total, @SourceType, @ExpenseId)
            ON CONFLICT (balance_id, source_type, expense_id) DO NOTHING
            """;

        var parameters = balance.Entries.Select(entry => new
        {
            Id = entry.Id.Value,
            BalanceId = entry.BalanceId.Value,
            Total = entry.Total.Amount,
            SourceType = entry.Source switch
            {
                BalanceEntry.BalanceEntrySource.Expense => "expense",
                _ => "none"
            },
            ExpenseId = (entry.Source as BalanceEntry.BalanceEntrySource.Expense)?.ExpenseId.Value
        });

        if (_transaction is not null)
        {
            foreach (var parameter in parameters)
            {
                await _transaction.Connection!.ExecuteAsync(sql, parameter, _transaction);
            }
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);
        foreach (var parameter in parameters)
        {
            await connection.ExecuteAsync(sql, parameter);
        }
    }

    public async Task<Balance?> GetAsync(BalanceId balanceId, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT
                id,
                name,
                balance,
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
