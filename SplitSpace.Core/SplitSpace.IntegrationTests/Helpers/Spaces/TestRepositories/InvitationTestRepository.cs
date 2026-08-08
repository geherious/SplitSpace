using SplitSpace.Spaces.Domain.Models.Aggregates.InvitationAggregate;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.IntegrationTests.Helpers.Spaces.TestRepositories;

public sealed class InvitationTestRepository
{
    private readonly IInvitationDomainRepository _domainRepository;

    public InvitationTestRepository(IInvitationDomainRepository domainRepository)
    {
        _domainRepository = domainRepository;
    }

    public Task SaveAsync(Invitation invitation, CancellationToken ct = default)
    {
        return _domainRepository.SaveAsync(invitation, ct);
    }

    public Task<Invitation?> GetAsync(InvitationId invitationId, CancellationToken ct = default)
    {
        return _domainRepository.GetAsync(invitationId, ct);
    }
}
