using SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.Finances.Domain.Models.Ids;

namespace SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;

public sealed class CategoryTestRepository
{
    private readonly ICategoryDomainRepository _domainRepository;
    private readonly ICategoryReadRepository _readRepository;

    public CategoryTestRepository(
        ICategoryDomainRepository domainRepository,
        ICategoryReadRepository readRepository)
    {
        _domainRepository = domainRepository;
        _readRepository = readRepository;
    }

    public Task SaveAsync(Category category, CancellationToken ct = default)
    {
        return _domainRepository.SaveAsync(category, ct);
    }

    public Task<Category?> GetAsync(CategoryId categoryId, CancellationToken ct = default)
    {
        return _domainRepository.GetAsync(categoryId, ct);
    }

    public Task<IReadOnlyCollection<Category>> GetBatchAsync(SpaceId spaceId, CancellationToken ct = default)
    {
        return _readRepository.GetBatchAsync(spaceId, ct);
    }
}
