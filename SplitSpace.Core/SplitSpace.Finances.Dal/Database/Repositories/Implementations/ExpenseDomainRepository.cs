using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class ExpenseDomainRepository : IExpenseDomainRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;

    public ExpenseDomainRepository(IFinanceDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task SaveAsync(Expense expense, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.CreateAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            await new ExpenseRepository(_connectionFactory, transaction).AddAsync(ToEntity(expense), cancellationToken);
            await new ExpenseSplitRepository(_connectionFactory, transaction).AddAsync(
                expense.ExpenseSplits.Select(ToEntity).ToArray(),
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static Entities.Expense ToEntity(Expense expense) => new()
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

    private static Entities.ExpenseSplit ToEntity(ExpenseSplit split) => new()
    {
        Id = split.Id.Value,
        ExpenseId = split.ExpenseId.Value,
        SpaceId = split.SpaceId.Value,
        UserId = split.UserId.Value,
        AmountToPay = split.AmountToPay.Amount
    };
}
