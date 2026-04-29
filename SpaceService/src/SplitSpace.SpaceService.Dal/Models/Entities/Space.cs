using SplitSpace.SpaceService.Common.Enums;

namespace SplitSpace.SpaceService.Dal.Models.Entities;

public record Space
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required SpaceType Type { get; set; }
    public required Guid OwnerId { get; set; }
    public required DateTimeOffset CreatedAt { get; init; }
}
