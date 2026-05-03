using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories.Implementations;

public class SettlementRepository : ISettlementRepository
{
    private readonly FinanceServiceDbContext _dbContext;

    public SettlementRepository(FinanceServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Settlement settlement)
    {
        await _dbContext.Settlements.AddAsync(settlement);
    }
}