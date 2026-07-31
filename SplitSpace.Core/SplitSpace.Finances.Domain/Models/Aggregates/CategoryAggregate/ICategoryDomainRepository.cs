using SplitSpace.Finances.Domain.Models.Ids;

namespace SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;

public interface ICategoryDomainRepository
{
    Task SaveAsync(Category category, CancellationToken ct = default);

    Task<Category?> GetAsync(CategoryId categoryId, CancellationToken ct = default);

    Task<IReadOnlyCollection<Category>> GetBatchAsync(SpaceId spaceId, CancellationToken ct = default);
}
