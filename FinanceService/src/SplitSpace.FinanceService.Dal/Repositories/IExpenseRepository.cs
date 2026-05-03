using SplitSpace.FinanceService.Dal.Models;
using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories;

public interface IExpenseRepository
{
    Task AddAsync(Expense expense);

    Task<IReadOnlyCollection<ExpenseSplitAggregate>> GetBatchAsync(Guid spaceId);
    
    Task<Expense?> GetAsync(Guid spaceId, Guid expenseId);
    
    Task<IReadOnlyCollection<ExpenseByCategory>> GetGroupedByCategory(Guid spaceId, DateTimeOffset fromDate, DateTimeOffset toDate);
}