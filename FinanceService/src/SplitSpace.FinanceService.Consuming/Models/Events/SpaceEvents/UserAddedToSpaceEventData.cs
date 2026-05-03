namespace SplitSpace.FinanceService.Consuming.Models.Events.SpaceEvents;

public record UsersAddedToSpaceEventData
{
    public required Guid SpaceId { get; init; }
    public required IReadOnlyCollection<Guid> UserIds { get; init; }
}
