namespace SplitSpace.Spaces.Dal.Database.Entities;

public sealed record InvitationEntity
{
    public required Guid Id { get; init; }
    public required Guid SpaceId { get; init; }
    public required Guid InvitedUserId { get; init; }
    public required Guid InvitedBy { get; init; }
    public required string Status { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
