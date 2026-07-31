using Mediator;
using SplitSpace.Finances.Dal.ClientFacades;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Dal.Database.Transactions;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Settlements.CreateSettlement;

public class CreateSettlementHandler : ICommandHandler<CreateSettlementCommand, Result>
{
    private readonly ITransactionProvider _transactionProvider;
    private readonly ISpacesServiceClientFacade _spacesServiceClientFacade;

    public CreateSettlementHandler(ITransactionProvider transactionProvider, ISpacesServiceClientFacade spacesServiceClientFacade)
    {
        _transactionProvider = transactionProvider;
        _spacesServiceClientFacade = spacesServiceClientFacade;
    }

    public async ValueTask<Result> Handle(CreateSettlementCommand command, CancellationToken ct)
    {
        if (command.Amount <= 0)
        {
            return Result.Failure(new Error(ErrorType.Validation, "Settlement amount must be greater than zero"));
        }

        var spaceIds = await _spacesServiceClientFacade.GetUserSpaceIdsAsync(new UserId(command.UserId), ct);
        if (spaceIds.Contains(new SpaceId(command.SpaceId)) is false)
        {
            return Result.Failure(new Error(ErrorType.NotFound, "Space not found"));
        }

        var memberUserIds = await _spacesServiceClientFacade.GetSpaceMemberUserIdsAsync(new SpaceId(command.SpaceId), ct);
        if (memberUserIds.Contains(new UserId(command.ToUserId)) is false)
        {
            return Result.Failure(new Error(ErrorType.NotFound, "Destination user not found"));
        }

        await using var tx = await _transactionProvider.BeginAsync(ct);
        try
        {
            var repos = tx.Repositories;

            var balance = await repos.BalanceRepository.GetAsync(command.BalanceId, ct);
            if (balance is null ||
                (balance.OwnerType == BalanceOwnerType.Personal && balance.OwnerId != command.UserId) ||
                (balance.OwnerType == BalanceOwnerType.Space && balance.OwnerId != command.SpaceId))
            {
                await tx.RollbackAsync(ct);
                return Result.Failure(new Error(ErrorType.NotFound, "Balance not found"));
            }

            if (balance.OwnerType == BalanceOwnerType.Space)
            {
                await tx.RollbackAsync(ct);
                return Result.Failure(new Error(ErrorType.FailedPrecondition, "Cannot pay split from space balance"));
            }

            var settlement = new Settlement
            {
                Id = Guid.CreateVersion7(),
                FromUserId = command.UserId,
                FromBalanceId = command.BalanceId,
                ToUserId = command.ToUserId,
                Amount = command.Amount,
                CreatedAt = DateTimeOffset.UtcNow
            };

            var debtFrom = command.UserId;
            var debtTo = command.ToUserId;
            var debtAmount = -command.Amount;

            if (debtFrom > debtTo)
            {
                (debtFrom, debtTo) = (debtTo, debtFrom);
                debtAmount *= -1;
            }

            var debt = new Debt
            {
                Id = Guid.CreateVersion7(),
                SpaceId = command.SpaceId,
                FromUserId = debtFrom,
                ToUserId = debtTo,
                Amount = debtAmount
            };

            await repos.SettlementRepository.AddAsync(settlement, ct);
            await repos.DebtRepository.AddOrUpdateAsync([debt], ct);
            await repos.BalanceRepository.UpdateAmountAsync(command.BalanceId, -command.Amount, ct);

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }

        return Result.Success();
    }
}
