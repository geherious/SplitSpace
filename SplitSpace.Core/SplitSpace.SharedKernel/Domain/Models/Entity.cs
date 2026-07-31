namespace SplitSpace.SharedKernel.Domain.Models;

public abstract record Entity<TId>
{
    public abstract TId Id { get; protected set; }
}