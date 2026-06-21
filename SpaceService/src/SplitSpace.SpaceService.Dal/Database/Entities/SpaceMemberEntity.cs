namespace SplitSpace.SpaceService.Dal.Database.Entities;

public sealed record SpaceMemberEntity
{
    public required Guid Id { get; init; }
    public required Guid SpaceId { get; init; }
    public required Guid UserId { get; init; }
    public required string Role { get; init; }
    public required DateTimeOffset JoinedAt { get; init; }
}
