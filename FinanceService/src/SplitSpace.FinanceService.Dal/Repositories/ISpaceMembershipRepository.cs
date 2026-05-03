using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories;

public interface ISpaceMembershipRepository
{
    Task<SpaceMembership?> GetAsync(Guid spaceId, Guid userId);
    
    Task<IReadOnlyCollection<SpaceMembership>> GetBatchAsync(Guid spaceId);
}
