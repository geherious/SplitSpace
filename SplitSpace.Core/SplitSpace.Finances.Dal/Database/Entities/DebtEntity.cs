namespace SplitSpace.Finances.Dal.Database.Entities;

public record DebtEntity
{
    public required Guid Id { get; init; }
    public required Guid SpaceId { get; init; }
    public required Guid FromUserId { get; init; }
    public required Guid ToUserId { get; init; }
    public required decimal Amount { get; init; }
}
