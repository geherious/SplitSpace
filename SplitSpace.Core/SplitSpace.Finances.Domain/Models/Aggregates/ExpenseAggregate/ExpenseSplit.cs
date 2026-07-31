using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;

public sealed record ExpenseSplit : Entity<ExpenseSplitId>
{
    public override ExpenseSplitId Id { get; protected set; }
    
    public ExpenseId ExpenseId { get; private set; }
    
    public SpaceId SpaceId { get; private set; }
    
    public UserId UserId { get; private set; }
    
    public Money AmountToPay { get; private set; }

    public SplitDetail Detail { get; private set; } = new SplitDetail.None();

    public abstract record SplitDetail
    {
        private SplitDetail() {}
        
        public sealed record None() : SplitDetail;
        
        public sealed record Equal() : SplitDetail;
        
        public sealed record Exact() : SplitDetail;
        
        public sealed record Percentage(decimal Value) : SplitDetail;
    }

    private ExpenseSplit(
        ExpenseSplitId id,
        ExpenseId expenseId,
        SpaceId spaceId,
        UserId userId,
        Money amountToPay,
        SplitDetail detail)
    {
        Id = id;
        ExpenseId = expenseId;
        SpaceId = spaceId;
        UserId = userId;
        AmountToPay = amountToPay;
        Detail = detail;
    }
    
    private ExpenseSplit() {}
    
    private ExpenseSplit Rehydrate(
        ExpenseSplitId id,
        ExpenseId expenseId,
        SpaceId spaceId,
        UserId userId,
        Money amountToPay,
        SplitDetail detail)
    {
        return new ExpenseSplit
        {
            Id = id,
            ExpenseId = expenseId,
            SpaceId = spaceId,
            UserId = userId,
            AmountToPay = amountToPay,
            Detail = detail,
        };
    }

    public static ExpenseSplit Create(
        ExpenseId expenseId,
        SpaceId spaceId,
        UserId userId,
        Money amountToPay,
        SplitDetail detail)
    {
        return new ExpenseSplit(
            ExpenseSplitId.New(),
            expenseId,
            spaceId,
            userId,
            amountToPay,
            detail);
    }
}
