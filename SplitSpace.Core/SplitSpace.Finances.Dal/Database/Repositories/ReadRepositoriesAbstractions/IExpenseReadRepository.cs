using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Dal.Database.Models;

namespace SplitSpace.Finances.Dal.Database.Repositories;

public interface IExpenseReadRepository
{
    Task<IReadOnlyCollection<ExpenseSplitAggregate>> GetBatchAsync(Guid spaceId, CancellationToken ct = default);
    
    Task<IReadOnlyCollection<ExpenseByCategory>> GetGroupedByCategory(
        Guid spaceId,
        DateTimeOffset fromDate,
        DateTimeOffset toDate,
        CancellationToken ct = default);
}
