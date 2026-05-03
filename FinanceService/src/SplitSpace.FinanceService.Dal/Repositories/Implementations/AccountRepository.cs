using Microsoft.EntityFrameworkCore;
using SplitSpace.FinanceService.Common.Enums;
using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories.Implementations;

public class AccountRepository : IAccountRepository
{
    private readonly FinanceServiceDbContext _dbContext;

    public AccountRepository(FinanceServiceDbContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(Account account)
    {
        await _dbContext.Accounts.AddAsync(account);
    }

    public async Task<Account?> GetAsync(Guid accountId)
    {
        return await  _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
    }

    public async Task<IReadOnlyCollection<Account>> GetSpaceAccountBatchAsync(Guid spaceId)
    {
        return await _dbContext.Accounts
            .Where(a => a.OwnerType == AccountOwnerType.Space && a.OwnerId == spaceId)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Account>> GetUserAccountBatchAsync(Guid userId)
    {
        return await _dbContext.Accounts
            .Where(a => a.OwnerType == AccountOwnerType.Personal && a.OwnerId == userId)
            .ToListAsync();
    }

    public async Task UpdateAmountAsync(Guid accountId, decimal amount)
    {
        await _dbContext.Accounts
            .Where(a => a.Id == accountId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(a => a.Balance, a => a.Balance + amount));
    }
}
