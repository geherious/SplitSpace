using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Logic.Features.Spaces.GetSpaceMembers;

public record GetSpaceMembersResultData(IReadOnlyCollection<GetSpaceMembersResultData.SpaceMember> Members)
{
    public record SpaceMember
    {
        public required UserId UserId { get; init; }
    }
}
