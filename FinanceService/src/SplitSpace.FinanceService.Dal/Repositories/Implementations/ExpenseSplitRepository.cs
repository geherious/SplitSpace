using Microsoft.EntityFrameworkCore;
using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories.Implementations;

public class ExpenseSplitRepository : IExpenseSplitRepository
{
    private readonly FinanceServiceDbContext _dbContext;

    public ExpenseSplitRepository(FinanceServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(IReadOnlyCollection<ExpenseSplit> expenseSplits)
    {
        await _dbContext.ExpenseSplits.AddRangeAsync(expenseSplits);
    }

    public async Task<ExpenseSplit?> GetAsync(Guid spaceId, Guid expenseId)
    {
        return await _dbContext.ExpenseSplits.FirstOrDefaultAsync(e => e.SpaceId == spaceId && e.Id == expenseId);
    }
}