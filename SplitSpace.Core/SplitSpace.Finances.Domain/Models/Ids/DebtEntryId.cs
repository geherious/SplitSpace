namespace SplitSpace.Finances.Domain.Models.Ids;

public record struct DebtEntryId(Guid Value)
{
    public static DebtEntryId New() => new(Guid.CreateVersion7());
}
