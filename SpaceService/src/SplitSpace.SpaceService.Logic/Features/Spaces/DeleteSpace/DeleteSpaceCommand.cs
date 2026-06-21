using Mediator;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Logic.Features.Spaces.DeleteSpace;

public record DeleteSpaceCommand(UserId UserId, SpaceId SpaceId) : ICommand<Result>;
