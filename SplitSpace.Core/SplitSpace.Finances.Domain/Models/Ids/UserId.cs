namespace SplitSpace.Finances.Domain.Models.Ids;

public readonly record struct UserId(Guid Value)
{
    public static UserId New() => new(Guid.CreateVersion7());
}
