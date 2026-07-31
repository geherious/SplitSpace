using SplitSpace.SharedKernel.Domain.Models;
using SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Domain.Models.Events;

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
