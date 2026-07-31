using Mediator;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Logic.Features.Spaces.DeleteSpace;

public record DeleteSpaceCommand(UserId UserId, SpaceId SpaceId) : ICommand<Result>;
