namespace SplitSpace.Auth.Domain.Models.Ids;

public readonly record struct RefreshTokenId(Guid Value)
{
    public static RefreshTokenId New() => new(Guid.CreateVersion7());
}
