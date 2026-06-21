using Microsoft.EntityFrameworkCore;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Dal.Repositories.Implementations;

public class SpaceDomainRepository : ISpaceDomainRepository
{
    private readonly SpaceServiceDbContext _dbContext;

    public SpaceDomainRepository(SpaceServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Space space, CancellationToken ct)
    {
        await _dbContext.Spaces.AddAsync(space, ct);
    }

    public Task<Space?> GetAsync(SpaceId spaceId, CancellationToken ct)
    {
        return _dbContext.Spaces.FirstOrDefaultAsync(s => s.Id == spaceId, ct);
    }

    public async Task DeleteAsync(Space space, CancellationToken ct)
    {
        _dbContext.Remove(space);
    }
}
