using Mediator;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Events;

public record ExpenseCreatedEvent(Expense Expense) : IDomainEvent, INotification;
