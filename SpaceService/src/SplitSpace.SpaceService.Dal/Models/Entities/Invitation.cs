using SplitSpace.SpaceService.Common.Enums;

namespace SplitSpace.SpaceService.Dal.Models.Entities;

public record Invitation
{
    public required Guid Id { get; init; }
    public required Guid SpaceId { get; init; }
    public required Guid InvitedUserId { get; init; }
    public required Guid InvitedBy { get; init; }
    public required InvitationStatus Status { get; set; }
    public required DateTime ExpiresAt { get; init; }
    public required DateTime CreatedAt { get; init; }
}
