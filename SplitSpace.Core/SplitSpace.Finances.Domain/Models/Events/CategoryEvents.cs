using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Events;

public record CategoryCreatedEvent(Category Category) : IDomainEvent;
