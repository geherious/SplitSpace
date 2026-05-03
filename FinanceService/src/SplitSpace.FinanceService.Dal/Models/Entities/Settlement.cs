namespace SplitSpace.FinanceService.Dal.Models.Entities;

public record Settlement
{
    public required Guid Id { get; init; }
    public required Guid FromUserId { get; init; }
    public required Guid FromAccountId { get; init; }
    public required Guid ToUserId { get; init; }
    public required decimal Amount { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
