using SplitSpace.SharedKernel.Domain.Models;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;

public sealed record SpaceMember : Entity<SpaceMemberId>
{
    public override SpaceMemberId Id { get; protected set; }

    public SpaceId SpaceId { get; private set; }

    public UserId UserId { get; private set; }

    public SpaceMemberRole Role { get; private set; }

    public DateTimeOffset JoinedAt { get; private set; }

    internal SpaceMember(
        SpaceMemberId id,
        UserId userId,
        SpaceId spaceId,
        SpaceMemberRole role,
        DateTimeOffset joinedAt)
    {
        Id = id;
        UserId = userId;
        SpaceId = spaceId;
        Role = role;
        JoinedAt = joinedAt;
    }

    public static SpaceMember Create(
        UserId userId,
        SpaceId spaceId,
        SpaceMemberRole role,
        DateTimeOffset joinedAt)
    {
        return new SpaceMember(
            SpaceMemberId.New(),
            userId,
            spaceId,
            role,
            joinedAt);
    }
}
