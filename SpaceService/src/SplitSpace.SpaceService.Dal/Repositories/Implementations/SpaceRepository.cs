using Microsoft.EntityFrameworkCore;
using SplitSpace.SpaceService.Dal;
using SplitSpace.SpaceService.Dal.Models.Entities;

namespace SplitSpace.SpaceService.Dal.Repositories.Implementations;

public class SpaceRepository : ISpaceRepository
{
    private readonly SpaceServiceDbContext _context;

    public SpaceRepository(SpaceServiceDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Space>> GetByMembershipAsync(Guid userId)
    {
        return await _context.Spaces
            .Join(_context.SpaceMemberships.Where(s => s.UserId == userId),
                space => space.Id,
                membership => membership.SpaceId,
                (space, membership) => space)
            .AsNoTracking()
            .ToArrayAsync();
    }

    public Task<Space?> GetAsync(Guid ownerId, Guid spaceId)
    {
        return _context.Spaces
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.OwnerId == ownerId && s.Id == spaceId);
    }

    public async Task CreateSpaceAsync(Space space)
    {
        await _context.Spaces.AddAsync(space);
    }

    public Task<int> DeleteSpaceAsync(Guid spaceId)
    {
        return _context.Spaces
            .Where(s => s.Id == spaceId)
            .ExecuteDeleteAsync();
    }
}
