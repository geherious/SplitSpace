namespace SplitSpace.FinanceService.Dal.Models.Entities;

public record Expense
{
    public required Guid Id { get; init; }
    public required Guid SpaceId { get; init; }
    public required Guid CreatedBy { get; init; }
    public required Guid CategoryId { get; init; }
    public required Guid AccountId { get; init; }
    public required decimal Amount { get; init; }
    public required string Description { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
