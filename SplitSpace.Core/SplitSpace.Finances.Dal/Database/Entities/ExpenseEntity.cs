namespace SplitSpace.Finances.Dal.Database.Entities;

public record ExpenseEntity
{
    public required Guid Id { get; init; }
    public required Guid SpaceId { get; init; }
    public required Guid CreatedBy { get; init; }
    public required Guid CategoryId { get; init; }
    public required Guid BalanceId { get; init; }
    public required decimal Amount { get; init; }
    public required string Description { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
