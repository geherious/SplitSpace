using SplitSpace.SpaceService.Common.Enums;

namespace SplitSpace.SpaceService.Dal.Models.Entities;

public record SpaceMembership
{
    public required Guid Id { get; init; }
    public required Guid SpaceId { get; init; }
    public required Guid UserId { get; init; }
    public required SpaceMembershipRole Role { get; set; }
    public required DateTimeOffset JoinedAt { get; init; }
}