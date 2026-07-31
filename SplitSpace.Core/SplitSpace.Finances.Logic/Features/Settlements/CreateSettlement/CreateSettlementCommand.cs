using Mediator;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Settlements.CreateSettlement;

public record CreateSettlementCommand(
    Guid SpaceId,
    Guid UserId,
    Guid BalanceId,
    decimal Amount,
    Guid ToUserId)
    : ICommand<Result>;
