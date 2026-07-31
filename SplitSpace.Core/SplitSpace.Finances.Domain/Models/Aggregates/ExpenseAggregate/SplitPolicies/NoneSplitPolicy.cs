using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate.SplitPolicies;

public class NoneSplitPolicy : IExpenseSplitPolicy
{
    public Result<IReadOnlyList<ExpenseSplit>> CalculateSplits(ExpenseId expenseId, SpaceId spaceId, Money total)
    {
        return Result.Success<IReadOnlyList<ExpenseSplit>>([]);
    }
}
