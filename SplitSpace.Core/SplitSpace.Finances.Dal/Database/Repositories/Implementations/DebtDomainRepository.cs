using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;
using SplitSpace.Finances.Domain.Models.Events;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class DebtDomainRepository : IDebtDomainRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public DebtDomainRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal DebtDomainRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task SaveAsync(Debt debt, CancellationToken cancellationToken)
    {
        if (_transaction is not null)
        {
            await ApplyEventsAsync(_transaction, debt, cancellationToken);
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            await ApplyEventsAsync(transaction, debt, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task ApplyEventsAsync(NpgsqlTransaction transaction, Debt debt, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in debt.DomainEvents)
        {
            switch (domainEvent)
            {
                case DebtEntryAddedEvent e:
                    await OnDebtEntryAddedAsync(transaction, e);
                    break;
            }
        }
        
        debt.ClearDomainEvents();
    }

    private static async Task OnDebtEntryAddedAsync(NpgsqlTransaction transaction, DebtEntryAddedEvent e)
    {
        const string upsertSql =
            """
            INSERT INTO debt (id, space_id, from_user_id, to_user_id, amount)
            VALUES (@Id, @SpaceId, @FromUserId, @ToUserId, @Amount)
            ON CONFLICT (space_id, from_user_id, to_user_id) DO UPDATE SET
                amount = EXCLUDED.amount
            """;

        const string insertEntrySql =
            """
            INSERT INTO debt_entry (id, debt_id, owned_by, owned_to, total, source_type, expense_id, split_id)
            VALUES (@Id, @DebtId, @OwnedBy, @OwnedTo, @Total, @SourceType, @ExpenseId, @SplitId)
            ON CONFLICT (debt_id, source_type, expense_id, split_id) DO NOTHING
            """;

        var connection = transaction.Connection!;

        var debtParameters = new
        {
            Id = e.Debt.Id.Value,
            SpaceId = e.Debt.SpaceId.Value,
            FromUserId = e.Debt.FromUserId.Value,
            ToUserId = e.Debt.ToUserId.Value,
            Amount = e.Debt.Total.Amount
        };

        await connection.ExecuteAsync(upsertSql, debtParameters, transaction);

        var expenseSource = e.Entry.Source as DebtEntry.DebtEntrySource.ExpenseSplit;
        var entryParameters = new
        {
            Id = e.Entry.Id.Value,
            DebtId = e.Entry.DebtId.Value,
            OwnedBy = e.Entry.OwnedBy.Value,
            OwnedTo = e.Entry.OwnedTo.Value,
            Total = e.Entry.Total.Amount,
            SourceType = expenseSource is null ? "none" : "expense_split",
            ExpenseId = expenseSource?.ExpenseId.Value,
            SplitId = expenseSource?.SplitId.Value
        };

        await connection.ExecuteAsync(insertEntrySql, entryParameters, transaction);
    }

    public async Task<Debt> GetOrCreate(Debt debt, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT
                id,
                space_id AS SpaceId,
                from_user_id AS FromUserId,
                to_user_id AS ToUserId,
                amount
            FROM debt
            WHERE space_id = @SpaceId AND from_user_id = @FromUserId AND to_user_id = @ToUserId
            """;

        var parameters = new
        {
            SpaceId = debt.SpaceId.Value,
            FromUserId = debt.FromUserId.Value,
            ToUserId = debt.ToUserId.Value
        };

        if (_transaction is not null)
        {
            var entity = await _transaction.Connection!.QuerySingleOrDefaultAsync<Entities.DebtEntity>(
                sql, parameters, _transaction);
            return entity is null ? debt : Map(entity);
        }

        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);
        var result = await connection.QuerySingleOrDefaultAsync<Entities.DebtEntity>(sql, parameters);
        return result is null ? debt : Map(result);
    }

    private static Debt Map(Entities.DebtEntity entity)
    {
        return Debt.Rehydrate(
            new DebtId(entity.Id),
            new SpaceId(entity.SpaceId),
            new UserId(entity.FromUserId),
            new UserId(entity.ToUserId),
            new Money(entity.Amount));
    }
}
