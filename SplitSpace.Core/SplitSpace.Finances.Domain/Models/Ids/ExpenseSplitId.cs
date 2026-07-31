namespace SplitSpace.Finances.Domain.Models.Ids;

public record struct ExpenseSplitId(Guid Value)
{
    public static ExpenseSplitId New() => new(Guid.CreateVersion7());
}
