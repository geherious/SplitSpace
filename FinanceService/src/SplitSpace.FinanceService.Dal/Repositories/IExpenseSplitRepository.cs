using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories;

public interface IExpenseSplitRepository
{
    Task AddAsync(IReadOnlyCollection<ExpenseSplit> expenseSplits);
    Task<ExpenseSplit?> GetAsync(Guid spaceId, Guid expenseId);
}
