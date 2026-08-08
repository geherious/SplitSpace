using SplitSpace.Finances.Dal.Database.Entities;

namespace SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;

public interface IDebtReadRepository
{
    Task<IReadOnlyCollection<DebtEntity>> GetBatchAsync(Guid spaceId, Guid userId, CancellationToken ct = default);
}
