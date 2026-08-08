using SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Events;

public record DebtEntryAddedEvent(Debt Debt, DebtEntry Entry) : IDomainEvent;
