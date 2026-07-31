using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;

public sealed record DebtEntry : Entity<DebtEntryId>
{
    public override DebtEntryId Id { get; protected set; }
    
    public DebtId DebtId { get; private set; }
    
    public UserId OwnedBy { get; private set; }
    
    public UserId OwnedTo { get; private set; }
    
    public Money Total { get; private set; }

    public DebtEntrySource Source { get; private set; } = new DebtEntrySource.None();

    private DebtEntry(DebtEntryId id, DebtId debtId, UserId ownedBy, UserId ownedTo, Money total, DebtEntrySource source)
    {
        Id = id;
        DebtId = debtId;
        OwnedBy = ownedBy;
        OwnedTo = ownedTo;
        Total = total;
        Source = source;
    }

    private DebtEntry() {}

    private DebtEntry Rehydrate(DebtEntryId id, DebtId debtId, UserId ownedBy, UserId ownedTo, Money total, DebtEntrySource source)
    {
        return new DebtEntry
        {
            Id = id,
            DebtId = debtId,
            OwnedBy = ownedBy,
            OwnedTo = ownedTo,
            Total = total,
            Source = source,
        };
    }

    public static DebtEntry Create(DebtId debtId, UserId ownedBy, UserId ownedTo, Money money, DebtEntrySource source)
    {
        return new DebtEntry(DebtEntryId.New(), debtId, ownedBy, ownedTo, money, source);
    }
    
    public abstract record DebtEntrySource
    {
        private DebtEntrySource() { }

        public sealed record None() : DebtEntrySource;
        
        public sealed record ExpenseSplit(ExpenseId ExpenseId, ExpenseSplitId SplitId) : DebtEntrySource; 
    }
}
