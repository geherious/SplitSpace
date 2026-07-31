namespace SplitSpace.Finances.Dal.Database.Entities;

public record Settlement
{
    public required Guid Id { get; init; }
    public required Guid FromUserId { get; init; }
    public required Guid FromBalanceId { get; init; }
    public required Guid ToUserId { get; init; }
    public required decimal Amount { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
