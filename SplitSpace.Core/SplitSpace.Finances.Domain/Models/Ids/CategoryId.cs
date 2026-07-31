namespace SplitSpace.Finances.Domain.Models.Ids;

public record struct CategoryId(Guid Value)
{
    public static CategoryId New() => new(Guid.CreateVersion7());
}
