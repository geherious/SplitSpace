namespace SplitSpace.SpaceService.Domain.Common;

public abstract record Entity<TId>
{
    public abstract TId Id { get; protected set; }
}