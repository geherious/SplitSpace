using Microsoft.EntityFrameworkCore;
using SplitSpace.SpaceService.Common.Enums;
using SplitSpace.SpaceService.Dal.Models.Entities;

namespace SplitSpace.SpaceService.Dal.Repositories.Implementations;

public class InvitationRepository : IInvitationRepository
{
    private readonly SpaceServiceDbContext _dbContext;

    public InvitationRepository(SpaceServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Invitation invitation)
    {
        await _dbContext.Invitations.AddAsync(invitation);
    }

    public Task<Invitation?> GetAsync(Guid invitationId)
    {
        return _dbContext.Invitations.FirstOrDefaultAsync(i => i.Id == invitationId);
    }

    public Task<Invitation?> GetAsync(Guid spaceId, Guid invitedUserId, InvitationStatus status)
    {
        return _dbContext.Invitations.FirstOrDefaultAsync(i => i.SpaceId == spaceId &&
                                                               i.InvitedUserId == invitedUserId &&
                                                               i.Status == status);
    }

    public async Task<int> AcceptInvitationAsync(Guid invitationId)
    {
        return await _dbContext.Invitations
            .Where(i => i.Id == invitationId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(i => i.Status, InvitationStatus.Accepted));
    }

    public async Task<int> RejectInvitationAsync(Guid invitationId)
    {
        return await _dbContext.Invitations
            .Where(i => i.Id == invitationId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(i => i.Status, InvitationStatus.Rejected));
    }
}
