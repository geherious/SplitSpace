using Mediator;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Spaces.Logic.Features.Spaces.GetSpaceMembers;
using SplitSpace.Spaces.Logic.Features.Spaces.GetSpaces;
using SpacesSpaceId = SplitSpace.Spaces.Domain.Models.Ids.SpaceId;
using SpacesUserId = SplitSpace.Spaces.Domain.Models.Ids.UserId;

namespace SplitSpace.Finances.Dal.ClientFacades.Implementations;

public class SpacesServiceClientFacade : ISpacesServiceClientFacade
{
    private readonly IMediator _mediator;

    public SpacesServiceClientFacade(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IReadOnlyCollection<SpaceId>> GetUserSpaceIdsAsync(UserId userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSpacesQuery(new SpacesUserId(userId.Value)), ct);

        if (result.IsSuccess is false || result.ResultValue is null)
        {
            return [];
        }

        return result.ResultValue.Spaces
            .Select(s => new SpaceId(s.Id.Value))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<UserId>> GetSpaceMemberUserIdsAsync(SpaceId spaceId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSpaceMembersQuery(new SpacesSpaceId(spaceId.Value)), ct);

        if (result.IsSuccess is false || result.ResultValue is null)
        {
            return [];
        }

        return result.ResultValue.Members
            .Select(m => new UserId(m.UserId.Value))
            .ToArray();
    }
}
