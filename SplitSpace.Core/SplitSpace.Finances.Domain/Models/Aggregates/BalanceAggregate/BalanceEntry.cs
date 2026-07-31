using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;

public sealed record BalanceEntry : Entity<BalanceEntryId>
{
    public override BalanceEntryId Id { get; protected set; }
    
    public BalanceId BalanceId { get; private set; }
    
    public Money Total { get; private set; }

    public BalanceEntrySource Source { get; private set; } = new BalanceEntrySource.None();

    private BalanceEntry(BalanceEntryId id, BalanceId balanceId, Money total, BalanceEntrySource source)
    {
        Id = id;
        BalanceId = balanceId;
        Total = total;
        Source = source;
    }
    
    private BalanceEntry() {}

    private BalanceEntry Rehydrate(BalanceEntryId id, BalanceId expenseId, BalanceEntrySource source)
    {
        return new BalanceEntry
        {
            Id = id,
            BalanceId = expenseId,
            Source = source,
        };
    }

    public static BalanceEntry Create(BalanceId balanceId, Money money, BalanceEntrySource source)
    {
        return new BalanceEntry(BalanceEntryId.New(), balanceId, money, source);
    }
    
    public abstract record BalanceEntrySource
    {
        private BalanceEntrySource() { }

        public sealed record None() : BalanceEntrySource;
        
        public sealed record Expense(ExpenseId ExpenseId) : BalanceEntrySource; 
    }
}
