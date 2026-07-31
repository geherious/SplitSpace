using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Domain.Models.Aggregates.InvitationAggregate;

public interface IInvitationDomainRepository
{
    Task<Invitation?> GetAsync(InvitationId invitationId, CancellationToken ct);

    Task SaveAsync(Invitation invitation, CancellationToken ct = default);
}
