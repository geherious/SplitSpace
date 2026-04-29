using Microsoft.EntityFrameworkCore;
using SplitSpace.SpaceService.Dal.Models.Entities;

namespace SplitSpace.SpaceService.Dal.Repositories.Implementations;

public class SpaceMembershipRepository : ISpaceMembershipRepository
{
    private readonly SpaceServiceDbContext _dbContext;

    public SpaceMembershipRepository(SpaceServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(SpaceMembership spaceMembership)
    {
        await _dbContext.SpaceMemberships.AddAsync(spaceMembership);
    }

    public async Task<SpaceMembership?> GetAsync(Guid spaceId, Guid userId)
    {
        return await _dbContext.SpaceMemberships.FirstOrDefaultAsync(s => s.SpaceId == spaceId && s.UserId == userId);
    }

    public async Task DeleteAsync(Guid spaceId)
    {
        await _dbContext.SpaceMemberships
            .Where(s => s.SpaceId == spaceId)
            .ExecuteDeleteAsync();
    }
}