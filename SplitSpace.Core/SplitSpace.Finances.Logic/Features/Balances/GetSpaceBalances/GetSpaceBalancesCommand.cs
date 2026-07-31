using Mediator;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Balances.GetSpaceBalances;

public record GetSpaceBalancesCommand(SpaceId SpaceId, UserId UserId)
    : IQuery<Result<GetSpaceBalancesResultData>>;
