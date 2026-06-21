using Mediator;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Logic.Features.Spaces.GetSpaces;

public record GetSpacesQuery(UserId UserId) : IQuery<Result<GetSpacesResultData>>;
