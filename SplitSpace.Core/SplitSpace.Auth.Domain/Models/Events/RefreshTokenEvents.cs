using SplitSpace.Auth.Domain.Models.Aggregates.RefreshTokenAggregate;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Auth.Domain.Models.Events;

public record RefreshTokenCreatedEvent(RefreshToken RefreshToken) : IDomainEvent;

public record RefreshTokenRevokedEvent(RefreshToken RefreshToken) : IDomainEvent;
