namespace SplitSpace.Spaces.Domain.Models.Ids;

public readonly record struct SpaceMemberId(Guid Value)
{
    public static SpaceMemberId New() => new(Guid.CreateVersion7());
}
