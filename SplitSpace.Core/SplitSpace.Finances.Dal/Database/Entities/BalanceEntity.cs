using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;

namespace SplitSpace.Finances.Dal.Database.Entities;

public record BalanceEntity
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required decimal Total { get; set; }
    public required BalanceOwnerType OwnerType { get; init; }
    public required Guid OwnerId { get; init; }
    public required Guid CreatedBy { get; init; }
}
