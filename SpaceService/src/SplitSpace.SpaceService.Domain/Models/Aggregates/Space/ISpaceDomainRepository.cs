using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

public interface ISpaceDomainRepository
{
    Task AddAsync(Space space, CancellationToken ct);
    
    Task<Space?> GetAsync(SpaceId spaceId, CancellationToken ct);
    
    Task DeleteAsync(Space space, CancellationToken ct);
}
