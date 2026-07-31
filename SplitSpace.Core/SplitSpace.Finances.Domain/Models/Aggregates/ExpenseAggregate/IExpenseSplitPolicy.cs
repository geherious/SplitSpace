using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;

public interface IExpenseSplitPolicy
{
    Result<IReadOnlyList<ExpenseSplit>> CalculateSplits(ExpenseId expenseId, SpaceId spaceId, Money total);
}
