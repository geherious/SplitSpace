using SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.IntegrationTests.Helpers.Spaces.TestRepositories;

public sealed class SpaceTestRepository
{
    private readonly ISpaceDomainRepository _domainRepository;

    public SpaceTestRepository(ISpaceDomainRepository domainRepository)
    {
        _domainRepository = domainRepository;
    }

    public Task SaveAsync(Space space, CancellationToken ct = default)
    {
        return _domainRepository.SaveAsync(space, ct);
    }

    public Task<Space?> GetAsync(SpaceId spaceId, CancellationToken ct = default)
    {
        return _domainRepository.GetAsync(spaceId, ct);
    }
}
