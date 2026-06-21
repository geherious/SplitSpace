using SplitSpace.SpaceService.Domain.Models.Aggregates.Invitation;
using SplitSpace.SpaceService.Domain.Models.Common;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Domain.Models.Events;

public record InvitationCreatedEvent(
    InvitationId Id,
    SpaceId SpaceId,
    UserId InvitedUserId,
    UserId InvitedBy,
    DateTimeOffset ExpiresAt,
    DateTimeOffset CreatedAt) : IDomainEvent;

public record InvitationAcceptedEvent(InvitationId Id) : IDomainEvent;

public record InvitationRejectedEvent(InvitationId Id) : IDomainEvent;
