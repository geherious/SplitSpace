using Mediator;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Balances.GetPersonalBalances;

public record GetPersonalBalancesCommand(UserId UserId)
    : IQuery<Result<GetPersonalBalancesResultData>>;
