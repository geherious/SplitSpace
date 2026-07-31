using Mediator;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Expenses.GetExpenses;

public record GetExpensesCommand(Guid SpaceId, Guid UserId)
    : IQuery<Result<GetExpensesResultData>>;
