using Mediator;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Balances.GetPersonalBalances;

public record GetPersonalBalancesCommand(Guid UserId)
    : IQuery<Result<GetPersonalBalancesResultData>>;
