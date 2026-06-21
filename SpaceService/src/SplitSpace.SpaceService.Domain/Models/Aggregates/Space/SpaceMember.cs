using SplitSpace.SpaceService.Domain.Common;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

public record SpaceMember : Entity<SpaceMemberId>
{
    public override SpaceMemberId Id { get; protected set; }
    
    public SpaceId SpaceId { get; private set; }
    
    public UserId UserId { get; private set; }
    
    public SpaceMemberRole Role { get; private set; }
    
    public DateTimeOffset JoinedAt { get; private set; }

    public static SpaceMember Create(
        UserId userId,
        SpaceId spaceId,
        SpaceMemberRole role,
        DateTimeOffset joinedAt)
    {
        return new SpaceMember
        {
            Id = SpaceMemberId.New(),
            UserId = userId,
            SpaceId =  spaceId,
            Role = role,
            JoinedAt = joinedAt
        };
    }
}
