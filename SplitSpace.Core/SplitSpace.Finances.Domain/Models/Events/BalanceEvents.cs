using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Events;

public record BalanceCreatedEvent(Balance Balance) : IDomainEvent;

public record BalanceAmountChangedEvent(Balance Balance, BalanceEntry Entry) : IDomainEvent;
