using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate.SplitPolicies;

public class ExactSplitPolicy : IExpenseSplitPolicy
{
    private readonly ExpenseSplitMethod.ExactAmount _splitData;
    
    public ExactSplitPolicy(ExpenseSplitMethod.ExactAmount splitData)
    {
        _splitData = splitData;
    }
    
    public Result<IReadOnlyList<ExpenseSplit>> CalculateSplits(ExpenseId expenseId, SpaceId spaceId, Money total)
    {
        if (_splitData.Participants.Count == 0)
        {
            return Result<IReadOnlyList<ExpenseSplit>>.Failure(
                new Error(ErrorType.FailedPrecondition, "Cannot calculate split policy for an empty list"));
        }

        var sum = _splitData.Participants.Sum(p => p.Amount.Amount);
        if (sum != total.Amount)
        {
            return Result<IReadOnlyList<ExpenseSplit>>.Failure(
                new Error(ErrorType.FailedPrecondition, "Total split amount does not match"));
        }
        
        var splits = new List<ExpenseSplit>(capacity: _splitData.Participants.Count);

        foreach (var participant in _splitData.Participants)
        {
            var split = ExpenseSplit.Create(
                expenseId,
                spaceId,
                participant.UserId,
                participant.Amount,
                new ExpenseSplit.SplitDetail.Exact());
            
            splits.Add(split);
        }
        
        return Result.Success<IReadOnlyList<ExpenseSplit>>(splits);
    }
}
