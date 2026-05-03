using Microsoft.EntityFrameworkCore;
using SplitSpace.FinanceService.Dal.Models;
using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories.Implementations;

public class ExpenseRepository : IExpenseRepository
{
    private readonly FinanceServiceDbContext _dbContext;

    public ExpenseRepository(FinanceServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Expense expense)
    {
        await _dbContext.Expenses.AddAsync(expense);
    }

    public async Task<IReadOnlyCollection<ExpenseSplitAggregate>> GetBatchAsync(Guid spaceId)
    {
        return await (
            from exp in _dbContext.Expenses
            join spl in _dbContext.ExpenseSplits
                on exp.Id equals spl.ExpenseId into splits
            join acc in _dbContext.Accounts
                on exp.AccountId equals acc.Id
            select new ExpenseSplitAggregate(exp, acc, splits.ToArray())
        ).ToListAsync();
    }

    public async Task<Expense?> GetAsync(Guid spaceId, Guid expenseId)
    {
        return await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == expenseId && e.SpaceId == spaceId);
    }

    public async Task<IReadOnlyCollection<ExpenseByCategory>> GetGroupedByCategory(
        Guid spaceId,
        DateTimeOffset fromDate,
        DateTimeOffset toDate)
    {
        return _dbContext.Expenses
            .Where(e => e.SpaceId == spaceId &&
                        e.CreatedAt >= fromDate &&
                        e.CreatedAt <= toDate)
            .GroupBy(e => e.CategoryId)
            .Select(g => new ExpenseByCategory(g.Key, g.Sum(i => i.Amount)))
            .ToArray();
    }
}