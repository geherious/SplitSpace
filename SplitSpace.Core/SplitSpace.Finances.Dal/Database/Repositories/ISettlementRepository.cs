using SplitSpace.Finances.Dal.Database.Entities;

namespace SplitSpace.Finances.Dal.Database.Repositories;

public interface ISettlementRepository
{
    Task AddAsync(Settlement settlement, CancellationToken ct = default);
}
