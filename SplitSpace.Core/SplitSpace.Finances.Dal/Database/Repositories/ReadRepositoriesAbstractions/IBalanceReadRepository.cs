using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Domain.Models.Ids;

namespace SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;

public interface IBalanceReadRepository
{
    Task<IReadOnlyCollection<BalanceEntity>> GetSpaceBalanceBatchAsync(SpaceId spaceId, CancellationToken ct = default);
    Task<IReadOnlyCollection<BalanceEntity>> GetUserBalanceBatchAsync(UserId userId, CancellationToken ct = default);
}
