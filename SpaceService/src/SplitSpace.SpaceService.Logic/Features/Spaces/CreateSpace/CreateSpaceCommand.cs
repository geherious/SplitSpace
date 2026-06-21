using Mediator;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Logic.Features.Spaces.CreateSpace;

public record CreateSpaceCommand(
    UserId UserId,
    string Name,
    SpaceType Type
) : ICommand<Result<CreateSpaceResultData>>;
