using SplitSpace.Finances.Domain.Models.Events;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;

public sealed record Debt : AggregateRoot<DebtId>
{
    public override DebtId Id { get; protected set; }
    
    public SpaceId SpaceId { get; private set; }
    
    public UserId FromUserId { get; private set; }
    
    public UserId ToUserId { get; private set; }
    
    public Money Total { get; private set; }

    private readonly List<DebtEntry> _entries = [];

    public IReadOnlyList<DebtEntry> Entries => _entries;

    private Debt(DebtId id, SpaceId spaceId, UserId fromUserId, UserId toUserId)
    {
        var (from, to, _) = Normalize(fromUserId, toUserId, new Money(0));

        Id = id;
        SpaceId = spaceId;
        FromUserId = from;
        ToUserId = to;
        Total = new Money(0);
    }
    
    private Debt() {}
    
    public static Debt Create(SpaceId spaceId, UserId fromUserId, UserId toUserId)
    {
        return new Debt(DebtId.New(), spaceId, fromUserId, toUserId);
    }

    public static Debt Rehydrate(DebtId id, SpaceId spaceId, UserId fromUserId, UserId toUserId, Money amount)
    {
        return new Debt
        {
            Id = id,
            SpaceId = spaceId,
            FromUserId = fromUserId,
            ToUserId = toUserId,
            Total = amount,
        };
    }

    public void ApplyEntry(DebtEntry entry)
    {
        var (from, to, normalizedAmount) = Normalize(entry.OwnedBy, entry.OwnedTo, entry.Total);

        if (from != FromUserId || to != ToUserId)
            Total -= normalizedAmount;
        else 
            Total += normalizedAmount;

        _entries.Add(entry);
        AddDomainEvent(new DebtEntryAddedEvent(this, entry));
    }
    
    private static (UserId From, UserId To, Money Amount) Normalize(UserId a, UserId b, Money amount)
    {
        return a.Value < b.Value
            ? (a, b, amount)
            : (b, a, amount.Negate());
    }
}
