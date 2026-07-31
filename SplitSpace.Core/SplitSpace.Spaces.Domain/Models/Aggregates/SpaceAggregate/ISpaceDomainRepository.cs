using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;

public interface ISpaceDomainRepository
{
    Task<Space?> GetAsync(SpaceId spaceId, CancellationToken ct);

    Task SaveAsync(Space space, CancellationToken ct = default);
}
