using Mediator;
using SplitSpace.Finances.Dal.ClientFacades;
using SplitSpace.Finances.Dal.Database.Repositories;
using SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Balances.GetSpaceBalances;

public class GetSpaceBalancesHandler : IQueryHandler<GetSpaceBalancesCommand, Result<GetSpaceBalancesResultData>>
{
    private readonly IBalanceReadRepository _balanceReadRepository;
    private readonly ISpacesServiceClientFacade _spacesServiceClientFacade;

    public GetSpaceBalancesHandler(IBalanceReadRepository balanceReadRepository, ISpacesServiceClientFacade spacesServiceClientFacade)
    {
        _balanceReadRepository = balanceReadRepository;
        _spacesServiceClientFacade = spacesServiceClientFacade;
    }

    public async ValueTask<Result<GetSpaceBalancesResultData>> Handle(GetSpaceBalancesCommand command, CancellationToken ct)
    {
        var spaceIds = await _spacesServiceClientFacade.GetUserSpaceIdsAsync(command.UserId, ct);
        if (spaceIds.Contains(command.SpaceId) is false)
        {
            return Result<GetSpaceBalancesResultData>.Failure(new Error(ErrorType.NotFound, "Space not found"));
        }

        var balances = await _balanceReadRepository.GetSpaceBalanceBatchAsync(command.SpaceId, ct);

        var result = balances
            .Select(a => new GetSpaceBalancesResultData.Balance
            {
                Id = a.Id,
                Name = a.Name,
                Total = a.Total,
                SpaceId = a.OwnerId
            })
            .ToArray();

        return Result<GetSpaceBalancesResultData>.Success(new GetSpaceBalancesResultData(result));
    }
}
