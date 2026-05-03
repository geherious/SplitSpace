using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories;

public interface IAccountRepository
{
    Task AddAsync(Account account);
    Task<Account?> GetAsync(Guid accountId);
    Task<IReadOnlyCollection<Account>> GetSpaceAccountBatchAsync(Guid spaceId);
    Task<IReadOnlyCollection<Account>> GetUserAccountBatchAsync(Guid userId);
    Task UpdateAmountAsync(Guid accountId, decimal amount);
}
