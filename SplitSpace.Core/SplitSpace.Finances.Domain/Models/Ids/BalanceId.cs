namespace SplitSpace.Finances.Domain.Models.Ids;

public record struct BalanceId(Guid Value)
{
    public static BalanceId New() => new(Guid.CreateVersion7());
}
