using SplitSpace.SpaceService.Dal.Models.Entities;

namespace SplitSpace.SpaceService.Dal.Repositories;

public interface ISpaceMembershipRepository
{
    Task AddAsync(SpaceMembership spaceMembership);
    
    Task<SpaceMembership?> GetAsync(Guid spaceId, Guid userId);
    
    Task DeleteAsync(Guid spaceId);
}
