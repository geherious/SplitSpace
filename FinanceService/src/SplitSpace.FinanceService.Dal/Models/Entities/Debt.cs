namespace SplitSpace.FinanceService.Dal.Models.Entities;

public record Debt
{
    public required Guid Id { get; init; }
    public required Guid SpaceId { get; init; }
    public required Guid FromUserId { get; init; }
    public required Guid ToUserId { get; init; }
    public required decimal Amount { get; init; }
}
