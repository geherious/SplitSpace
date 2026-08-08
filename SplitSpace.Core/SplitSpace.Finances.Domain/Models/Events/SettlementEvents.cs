using SplitSpace.Finances.Domain.Models.Aggregates.SettlementAggregate;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Events;

public record SettlementCreatedEvent(Settlement Settlement) : IDomainEvent;
