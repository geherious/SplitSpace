using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Domain.Models.Aggregates.Invitation;

public interface IInvitationDomainRepository
{
    Task<Invitation?> GetAsync(InvitationId invitationId, CancellationToken ct);

    Task SaveAsync(Invitation invitation, CancellationToken ct = default);
}
