using Mediator;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Logic.Features.Spaces.GetSpaces;

public record GetSpacesQuery(UserId UserId) : IQuery<Result<GetSpacesResultData>>;
