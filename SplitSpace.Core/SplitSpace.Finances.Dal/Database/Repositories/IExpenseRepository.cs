using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Dal.Database.Models;

namespace SplitSpace.Finances.Dal.Database.Repositories;

public interface IExpenseRepository
{
    Task AddAsync(Expense expense, CancellationToken ct = default);

    Task<IReadOnlyCollection<ExpenseSplitAggregate>> GetBatchAsync(Guid spaceId, CancellationToken ct = default);

    Task<Expense?> GetAsync(Guid spaceId, Guid expenseId, CancellationToken ct = default);

    Task<IReadOnlyCollection<ExpenseByCategory>> GetGroupedByCategory(
        Guid spaceId,
        DateTimeOffset fromDate,
        DateTimeOffset toDate,
        CancellationToken ct = default);
}
