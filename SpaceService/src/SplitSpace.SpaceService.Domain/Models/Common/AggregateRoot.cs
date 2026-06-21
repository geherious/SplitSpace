using SplitSpace.SpaceService.Domain.Common;

namespace SplitSpace.SpaceService.Domain.Models.Common;

public abstract record AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
