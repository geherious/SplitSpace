using Mediator;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Logic.Features.Spaces.CreateSpace;

public record CreateSpaceCommand(
    UserId UserId,
    string Name,
    SpaceType Type
) : ICommand<Result<CreateSpaceResultData>>;
