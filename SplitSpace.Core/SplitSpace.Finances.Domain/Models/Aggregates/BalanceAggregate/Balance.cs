using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Domain.Models;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;

public sealed record Balance : AggregateRoot<BalanceId>
{
    public override BalanceId Id { get; protected set; }
    
    public string Name { get; private set; } = string.Empty;
    
    public Money Total { get; private set; }
    
    public BalanceOwnerType OwnerType { get; private set; }
    
    public Guid OwnerId { get; private set; }
    
    public Guid CreatedBy { get; private set; }

    private readonly List<BalanceEntry> _entries = [];

    public IReadOnlyList<BalanceEntry> Entries => _entries;

    private Balance(
        BalanceId id,
        string name,
        Money total,
        BalanceOwnerType ownerType,
        Guid ownerId,
        Guid createdBy)
    {
        Id = id;
        Name = name;
        Total = total;
        OwnerType = ownerType;
        OwnerId = ownerId;
        CreatedBy = createdBy;
    }
    
    private Balance() { }

    public static Balance Rehydrate(
        BalanceId id,
        string name,
        Money amount,
        BalanceOwnerType ownerType,
        Guid ownerId,
        Guid createdBy)
    {
        return new Balance
        {
            Id = id,
            Name = name,
            Total = amount,
            OwnerType = ownerType,
            OwnerId = ownerId,
            CreatedBy = createdBy,
        };
    }

    public static Result<Balance> CreatePersonal(string name, UserId ownerId, UserId createdBy)
    {
        if (ownerId != createdBy)
        {
            return Result<Balance>.Failure(new Error(ErrorType.FailedPrecondition, "Owner id should equal the user id"));
        }

        return Result.Success(new Balance(
            BalanceId.New(),
            name,
            total: new Money(0),
            BalanceOwnerType.Personal,
            ownerId.Value,
            createdBy.Value));
    }
    
    public static Balance CreateSpace(string name, SpaceId spaceId, UserId createdBy)
    {
        return new Balance(
            BalanceId.New(),
            name,
            total: new Money(0),
            BalanceOwnerType.Space,
            spaceId.Value,
            createdBy.Value);
    }

    public bool IsOwnedByUser(UserId userId)
    {
        return OwnerType == BalanceOwnerType.Personal && OwnerId == userId.Value;
    }

    public bool IsOwnedBySpace(SpaceId spaceId)
    {
        return OwnerType == BalanceOwnerType.Space && OwnerId == spaceId.Value;
    }
    
    public Result ApplyEntry(BalanceEntry entry)
    {
        Total = new Money(entry.Total.Amount);
        _entries.Add(entry);
        return Result.Success();
    }
}
