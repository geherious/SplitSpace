using SplitSpace.Finances.Domain.Models.Ids;

namespace SplitSpace.Finances.Dal.ClientFacades;

public interface ISpacesServiceClientFacade
{
    Task<IReadOnlyCollection<SpaceId>> GetUserSpaceIdsAsync(UserId userId, CancellationToken ct = default);

    Task<IReadOnlyCollection<UserId>> GetSpaceMemberUserIdsAsync(SpaceId spaceId, CancellationToken ct = default);
}
