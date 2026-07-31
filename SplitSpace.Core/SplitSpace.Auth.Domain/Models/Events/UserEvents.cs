using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Auth.Domain.Models.Events;

public record UserCreatedEvent(User User) : IDomainEvent;
