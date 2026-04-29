using SplitSpace.SpaceService.Common.Enums;

namespace SplitSpace.SpaceService.Logic.Models.Results;

public record GetSpacesResultData(IReadOnlyCollection<GetSpacesResultData.Space> Spaces)
{
    public record Space(Guid Id, string Name, SpaceType SpaceType, SpaceMembershipRole Role);
}
