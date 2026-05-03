namespace SplitSpace.SpaceService.Producing.Models.SpaceEvents;

public record UsersAddedToSpaceEventData
{
    public required Guid SpaceId { get; init; }
    public required IReadOnlyCollection<Guid> UserIds { get; init; }
}
