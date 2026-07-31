using SplitSpace.Finances.Dal.Database.Entities;

namespace SplitSpace.Finances.Dal.Database.Repositories;

public interface IBalanceRepository
{
    Task AddAsync(BalanceEntity balanceEntity, CancellationToken ct = default);
    Task<BalanceEntity?> GetAsync(Guid balanceId, CancellationToken ct = default);
    Task<IReadOnlyCollection<BalanceEntity>> GetSpaceBalanceBatchAsync(Guid spaceId, CancellationToken ct = default);
    Task<IReadOnlyCollection<BalanceEntity>> GetUserbalanceBatchAsync(Guid userId, CancellationToken ct = default);
    Task UpdateAmountAsync(Guid balanceId, decimal amount, CancellationToken ct = default);
}
