using SplitSpace.Finances.Dal.Database.Entities;

namespace SplitSpace.Finances.Dal.Database.Repositories;

public interface IExpenseSplitRepository
{
    Task AddAsync(IReadOnlyCollection<ExpenseSplit> expenseSplits, CancellationToken ct = default);

    Task<ExpenseSplit?> GetAsync(Guid spaceId, Guid expenseId, CancellationToken ct = default);
}
