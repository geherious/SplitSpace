namespace SplitSpace.SpaceService.Dal.Database.Entities;

public sealed record SpaceEntity
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required Guid OwnerId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
