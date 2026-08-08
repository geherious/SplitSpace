using Mediator;
using SplitSpace.Finances.Dal.ClientFacades;
using SplitSpace.Finances.Dal.Database.Transactions;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.SettlementAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Settlements.CreateSettlement;

public class CreateSettlementHandler : ICommandHandler<CreateSettlementCommand, Result>
{
    private readonly ITransactionProvider _transactionProvider;
    private readonly ISpacesServiceClientFacade _spacesServiceClientFacade;
    private readonly TimeProvider _timeProvider;

    public CreateSettlementHandler(
        ITransactionProvider transactionProvider,
        ISpacesServiceClientFacade spacesServiceClientFacade,
        TimeProvider timeProvider)
    {
        _transactionProvider = transactionProvider;
        _spacesServiceClientFacade = spacesServiceClientFacade;
        _timeProvider = timeProvider;
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

            var balance = await repos.BalanceDomainRepository.GetAsync(new BalanceId(command.BalanceId), ct);
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

            var settlement = Settlement.Create(
                new UserId(command.UserId),
                new BalanceId(command.BalanceId),
                new UserId(command.ToUserId),
                new Money(command.Amount),
                _timeProvider.GetUtcNow());

            if (settlement.IsSuccess is false)
            {
                await tx.RollbackAsync(ct);
                return Result.Failure(settlement.Errors);
            }

            balance.ApplyEntry(BalanceEntry.Create(
                balance.Id,
                new Money(balance.Total.Amount - command.Amount),
                new BalanceEntry.BalanceEntrySource.None()));

            var debtToCreate = Debt.Create(
                new SpaceId(command.SpaceId),
                new UserId(command.UserId),
                new UserId(command.ToUserId));
            var debt = await repos.DebtDomainRepository.GetOrCreate(debtToCreate, ct);

            debt.ApplyEntry(DebtEntry.Create(
                debt.Id,
                new UserId(command.UserId),
                new UserId(command.ToUserId),
                new Money(-command.Amount),
                new DebtEntry.DebtEntrySource.None()));

            await repos.BalanceDomainRepository.SaveAsync(balance, ct);
            await repos.DebtDomainRepository.SaveAsync(debt, ct);
            await repos.SettlementDomainRepository.SaveAsync(settlement.ResultValue, ct);

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
