using SplitSpace.Finances.Dal.Database.Entities;

namespace SplitSpace.Finances.Dal.Database.Repositories;

public interface IDebtRepository
{
    Task AddOrUpdateAsync(IReadOnlyCollection<Debt> debts, CancellationToken ct = default);

    Task<IReadOnlyCollection<Debt>> GetBatchAsync(Guid spaceId, Guid userId, CancellationToken ct = default);
}
