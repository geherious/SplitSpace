using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Domain.Models.Aggregates.Invitation;

public interface IInvitationDomainRepository
{
    Task AddAsync(Invitation invitation, CancellationToken ct);
    
    Task<Invitation?> GetAsync(InvitationId invitationId, CancellationToken ct);
}
