using Mediator;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Logic.Features.Spaces.GetSpaceMembers;

public record GetSpaceMembersQuery(SpaceId SpaceId) : IQuery<Result<GetSpaceMembersResultData>>;
