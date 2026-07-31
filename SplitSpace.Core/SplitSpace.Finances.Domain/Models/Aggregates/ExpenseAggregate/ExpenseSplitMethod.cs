using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;

namespace SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;

public abstract record ExpenseSplitMethod
{
    private ExpenseSplitMethod()
    {
    }
        
    public sealed record None() : ExpenseSplitMethod;

    public sealed record EqualSplit() : ExpenseSplitMethod
    {
        public required IReadOnlyList<SplitParticipant> Participants { get; init; }
            
        public record SplitParticipant(UserId UserId);
    }

    public sealed record ExactAmount() : ExpenseSplitMethod
    {
        public required IReadOnlyList<SplitParticipant> Participants { get; init; }
            
        public record SplitParticipant(UserId UserId, Money Amount);
    }
        
    public sealed record Percentages() : ExpenseSplitMethod
    {
        public required IReadOnlyList<SplitParticipant> Participants { get; init; }
            
        public record SplitParticipant(UserId UserId, decimal Percentage);
    }
}
