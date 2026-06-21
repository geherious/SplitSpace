using Microsoft.EntityFrameworkCore;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Invitation;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Dal.Repositories.Implementations;

public class InvitationDomainRepository : IInvitationDomainRepository
{
    private readonly SpaceServiceDbContext _dbContext;

    public InvitationDomainRepository(SpaceServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Invitation invitation, CancellationToken ct)
    {
        await _dbContext.Invitations.AddAsync(invitation, ct);
    }

    public Task<Invitation?> GetAsync(InvitationId invitationId, CancellationToken ct)
    {
        return _dbContext.Invitations.FirstOrDefaultAsync(i => i.Id == invitationId, ct);
    }
}