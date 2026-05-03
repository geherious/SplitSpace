using System.Text.Json;

namespace SplitSpace.FinanceService.Consuming.Models.Events.SpaceEvents;

public record SpaceEvent
{
    public required string Type { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public required Guid SpaceId { get; init; }
    public required JsonElement Data { get; init; }
}
