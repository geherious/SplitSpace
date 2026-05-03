namespace SplitSpace.FinanceService.Dal.Models.Entities;

public record Tag
{
    public required Guid Id { get; init; }
    public required Guid SpaceId { get; init; }
    public required string Name { get; init; }
}
