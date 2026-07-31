namespace SplitSpace.Finances.Domain.Models.Ids;

public record struct DebtId(Guid Value)
{
    public static DebtId New() => new(Guid.CreateVersion7());
}
