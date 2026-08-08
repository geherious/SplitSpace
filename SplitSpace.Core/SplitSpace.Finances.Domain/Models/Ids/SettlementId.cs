namespace SplitSpace.Finances.Domain.Models.Ids;

public record struct SettlementId(Guid Value)
{
    public static SettlementId New() => new(Guid.CreateVersion7());
}
