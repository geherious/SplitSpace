using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

public interface ISpaceDomainRepository
{
    Task<Space?> GetAsync(SpaceId spaceId, CancellationToken ct);

    Task SaveAsync(Space space, CancellationToken ct = default);
}
