namespace SplitSpace.FinanceService.Dal.Models.Entities;

public record Category
{
    public required Guid Id { get; init; }
    public required Guid SpaceId { get; init; }
    public required string Name { get; init; }
    public Guid? ParentId { get; init; }
    public decimal? Limit { get; init; }
}
