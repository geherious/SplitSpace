using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;
using SplitSpace.SpaceService.Domain.Models.Common;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Domain.Models.Events;

public record SpaceCreatedEvent(
    SpaceId Id,
    string Name,
    SpaceType Type,
    UserId OwnerId,
    DateTimeOffset CreatedAt) : IDomainEvent;

public record SpaceMemberAddedEvent(
    SpaceMemberId MemberId,
    SpaceId SpaceId,
    UserId UserId,
    SpaceMemberRole Role,
    DateTimeOffset JoinedAt) : IDomainEvent;

public record SpaceDeletedEvent(SpaceId Id) : IDomainEvent;
