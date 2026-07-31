using Mediator;
using SplitSpace.Finances.Dal.ClientFacades;
using SplitSpace.Finances.Dal.Database.Transactions;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Balances.AddBalance;

public class AddBalanceHandler : ICommandHandler<AddBalanceCommand, Result<AddBalanceResultData>>
{
    private readonly ITransactionProvider _transactionProvider;
    private readonly IBalanceDomainRepository _balanceDomainRepository;
    private readonly ISpacesServiceClientFacade _spacesServiceClientFacade;

    public AddBalanceHandler(ITransactionProvider transactionProvider, ISpacesServiceClientFacade spacesServiceClientFacade, IBalanceDomainRepository balanceDomainRepository)
    {
        _transactionProvider = transactionProvider;
        _spacesServiceClientFacade = spacesServiceClientFacade;
        _balanceDomainRepository = balanceDomainRepository;
    }

    public async ValueTask<Result<AddBalanceResultData>> Handle(AddBalanceCommand command, CancellationToken ct)
    {
        Result<Balance> createdBalance;
        switch (command.BalanceOwnerType)
        {
            case BalanceOwnerType.Personal:
                createdBalance = Balance.CreatePersonal(
                    command.Name,
                    new UserId(command.OwnerId),
                    command.UserId);
                break;
            case BalanceOwnerType.Space:
                var spaceIds = await _spacesServiceClientFacade.GetUserSpaceIdsAsync(command.UserId, ct);
                if (spaceIds.Contains(new SpaceId(command.OwnerId)) is false)
                {
                    return Result<AddBalanceResultData>.Failure(new Error(ErrorType.NotFound, "Space not found"));
                }

                createdBalance = Result.Success(Balance.CreateSpace(
                    command.Name,
                    new SpaceId(command.OwnerId),
                    command.UserId));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(command.BalanceOwnerType));
        }

        if (createdBalance.IsSuccess is false)
        {
            return Result<AddBalanceResultData>.Failure(createdBalance.Errors);
        }

        await _balanceDomainRepository.SaveAsync(createdBalance.ResultValue, ct);

        return Result<AddBalanceResultData>.Success(new AddBalanceResultData(createdBalance.ResultValue.Id));
    }
}
