namespace SplitSpace.Finances.Domain.Models.Ids;

public record struct BalanceEntryId(Guid Value)
{
    public static BalanceEntryId New() => new(Guid.CreateVersion7());
}
