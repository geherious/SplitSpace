using SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Logic.Features.Spaces.GetSpaces;

public record GetSpacesResultData(IReadOnlyCollection<GetSpacesResultData.Space> Spaces)
{
    public record Space
    {
        public required SpaceId Id { get; init; }
        public required string Name { get; init; }
        public required SpaceType SpaceType { get; init; }
        public required SpaceMemberRole Role { get; init; }
    }
}
