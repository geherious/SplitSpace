namespace SplitSpace.Finances.Domain.Models.Ids;

public record struct ExpenseId(Guid Value)
{
    public static ExpenseId New() => new(Guid.CreateVersion7());
}
