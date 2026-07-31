using Mediator;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Expenses.AddExpense;

public record AddExpenseCommand(
    SpaceId SpaceId,
    UserId UserId,
    CategoryId CategoryId,
    BalanceId BalanceId,
    decimal Amount,
    string Description,
    ExpenseSplitMethod Split,
    DateTimeOffset CreatedAt)
    : ICommand<Result<AddExpenseResultData>>;
