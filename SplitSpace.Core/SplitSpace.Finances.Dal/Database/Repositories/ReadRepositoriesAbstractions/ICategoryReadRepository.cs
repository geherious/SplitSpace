using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.Finances.Domain.Models.Ids;

namespace SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;

public interface ICategoryReadRepository
{
    Task<IReadOnlyCollection<Category>> GetBatchAsync(SpaceId spaceId, CancellationToken ct = default);
}
