using Mediator;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Balances.AddBalance;

public record AddBalanceCommand(
    UserId UserId,
    Guid OwnerId,
    BalanceOwnerType BalanceOwnerType,
    string Name,
    decimal Balance)
    : ICommand<Result<AddBalanceResultData>>;
