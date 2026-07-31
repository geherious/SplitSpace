using SplitSpace.SharedKernel.Domain.Models;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Domain.Models.Events;

public record InvitationCreatedEvent(
    InvitationId Id,
    SpaceId SpaceId,
    UserId InvitedUserId,
    UserId InvitedBy,
    DateTimeOffset ExpiresAt,
    DateTimeOffset CreatedAt) : IDomainEvent;

public record InvitationAcceptedEvent(InvitationId Id) : IDomainEvent;

public record InvitationRejectedEvent(InvitationId Id) : IDomainEvent;
