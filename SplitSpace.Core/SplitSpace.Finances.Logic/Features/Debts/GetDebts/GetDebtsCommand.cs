using Mediator;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Debts.GetDebts;

public record GetDebtsCommand(Guid SpaceId, Guid UserId)
    : IQuery<Result<GetDebtsResultData>>;
