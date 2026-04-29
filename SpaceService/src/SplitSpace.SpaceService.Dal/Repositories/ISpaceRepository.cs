using SplitSpace.SpaceService.Dal.Models.Entities;

namespace SplitSpace.SpaceService.Dal.Repositories;

public interface ISpaceRepository
{
    Task<IReadOnlyCollection<Space>> GetByMembershipAsync(Guid userId);
    Task<Space?> GetAsync(Guid ownerId, Guid spaceId);
    Task CreateSpaceAsync(Space space);
    Task<int> DeleteSpaceAsync(Guid spaceId);
}
