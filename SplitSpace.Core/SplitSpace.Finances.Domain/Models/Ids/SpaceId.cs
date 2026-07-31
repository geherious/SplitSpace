namespace SplitSpace.Finances.Domain.Models.Ids;

public readonly record struct SpaceId(Guid Value)
{
    public static SpaceId New() => new(Guid.CreateVersion7());
}
