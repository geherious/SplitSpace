using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate.SplitPolicies;

public class EqualSplitPolicy : IExpenseSplitPolicy
{
    private readonly ExpenseSplitMethod.EqualSplit _splitData;
    
    public EqualSplitPolicy(ExpenseSplitMethod.EqualSplit splitData)
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
        
        var splits = new List<ExpenseSplit>(capacity: _splitData.Participants.Count);

        foreach (var participant in _splitData.Participants)
        {
            var amountToPay = total.Amount / _splitData.Participants.Count;
            var split = ExpenseSplit.Create(
                expenseId,
                spaceId,
                participant.UserId,
                new Money(amountToPay),
                new ExpenseSplit.SplitDetail.Equal());
            
            splits.Add(split);
        }
        
        return Result.Success<IReadOnlyList<ExpenseSplit>>(splits);
    }
}
