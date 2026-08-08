using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;

public interface ICategoryDomainRepository
{
    Task SaveAsync(Category category, CancellationToken ct = default);

    Task<Category?> GetAsync(CategoryId categoryId, CancellationToken ct = default);
}
