using Microsoft.EntityFrameworkCore;
using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories.Implementations;

public class SpaceMembershipRepository : ISpaceMembershipRepository
{
    private readonly FinanceServiceDbContext _dbContext;

    public SpaceMembershipRepository(FinanceServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddBatchAsync(IReadOnlyCollection<SpaceMembership> spaceMemberships)
    {
        await _dbContext.SpaceMemberships.AddRangeAsync(spaceMemberships);
    }

    public async Task<SpaceMembership?> GetAsync(Guid spaceId, Guid userId)
    {
        return await _dbContext.SpaceMemberships.FirstOrDefaultAsync(m => m.SpaceId == spaceId && m.UserId == userId);
    }

    public async Task<IReadOnlyCollection<SpaceMembership>> GetBatchAsync(Guid spaceId)
    {
        return await _dbContext.SpaceMemberships.Where(m => m.SpaceId == spaceId).ToListAsync();
    }
}
