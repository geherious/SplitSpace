using Mediator;
using SplitSpace.Finances.Dal.Database.Repositories;
using SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Balances.GetPersonalBalances;

public class GetPersonalBalancesHandler : IQueryHandler<GetPersonalBalancesCommand, Result<GetPersonalBalancesResultData>>
{
    private readonly IBalanceReadRepository _balanceReadRepository;

    public GetPersonalBalancesHandler(IBalanceReadRepository balanceReadRepository)
    {
        _balanceReadRepository = balanceReadRepository;
    }

    public async ValueTask<Result<GetPersonalBalancesResultData>> Handle(GetPersonalBalancesCommand command, CancellationToken ct)
    {
        var accounts = await _balanceReadRepository.GetUserBalanceBatchAsync(command.UserId, ct);

        var result = accounts
            .Select(a => new GetPersonalBalancesResultData.Account
            {
                Id = a.Id,
                Name = a.Name,
                Balance = a.Total,
                UserId = a.OwnerId
            })
            .ToArray();

        return Result<GetPersonalBalancesResultData>.Success(new GetPersonalBalancesResultData(result));
    }
}
