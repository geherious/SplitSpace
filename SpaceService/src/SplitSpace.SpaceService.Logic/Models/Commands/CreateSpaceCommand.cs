using SplitSpace.SpaceService.Common.Enums;

namespace SplitSpace.SpaceService.Logic.Models.Commands;

public record CreateSpaceCommand(
    string UserId,
    string Name,
    SpaceType Type
);
