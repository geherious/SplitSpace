namespace SplitSpace.SpaceService.Producing.Models.SpaceEvents;

public record SpaceEvent<TData>
{
    public string Type { get; set; } = typeof(TData).Name;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public required Guid SpaceId { get; init; }
    public required TData Data { get; init; }
}
