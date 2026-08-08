using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;
using SplitSpace.Finances.Domain.Models.Events;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class ExpenseDomainRepository : IExpenseDomainRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public ExpenseDomainRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal ExpenseDomainRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task SaveAsync(Expense expense, CancellationToken cancellationToken)
    {
        if (_transaction is not null)
        {
            await ApplyEventsAsync(_transaction, expense, cancellationToken);
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            await ApplyEventsAsync(transaction, expense, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task ApplyEventsAsync(NpgsqlTransaction transaction, Expense expense, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in expense.DomainEvents)
        {
            switch (domainEvent)
            {
                case ExpenseCreatedEvent e:
                    await OnExpenseCreatedAsync(transaction, e);
                    break;
            }
        }
        
        expense.ClearDomainEvents();
    }

    private static async Task OnExpenseCreatedAsync(NpgsqlTransaction transaction, ExpenseCreatedEvent e)
    {
        const string expenseSql =
            """
            INSERT INTO expense (id, space_id, created_by, category_id, balance_id, amount, description, created_at)
            VALUES (@Id, @SpaceId, @CreatedBy, @CategoryId, @BalanceId, @Amount, @Description, @CreatedAt)
            """;

        const string splitSql =
            """
            INSERT INTO expense_split (id, expense_id, space_id, user_id, amount_to_pay)
            VALUES (@Id, @ExpenseId, @SpaceId, @UserId, @AmountToPay)
            """;

        var connection = transaction.Connection!;

        await connection.ExecuteAsync(
            expenseSql,
            ToEntity(e.Expense),
            transaction);

        foreach (var split in e.Expense.ExpenseSplits)
        {
            await connection.ExecuteAsync(
                splitSql,
                ToEntity(split),
                transaction);
        }
    }

    private static Entities.ExpenseEntity ToEntity(Expense expense) => new()
    {
        Id = expense.Id.Value,
        SpaceId = expense.SpaceId.Value,
        CreatedBy = expense.CreatedBy.Value,
        CategoryId = expense.CategoryId.Value,
        BalanceId = expense.BalanceId.Value,
        Amount = expense.Amount.Amount,
        Description = expense.Description,
        CreatedAt = expense.CreatedAt
    };

    private static Entities.ExpenseSplitEntity ToEntity(ExpenseSplit split) => new()
    {
        Id = split.Id.Value,
        ExpenseId = split.ExpenseId.Value,
        SpaceId = split.SpaceId.Value,
        UserId = split.UserId.Value,
        AmountToPay = split.AmountToPay.Amount
    };
}
