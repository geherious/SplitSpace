using Mediator;
using SplitSpace.Finances.Dal.Database.Repositories;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Balances.GetPersonalBalances;

public class GetPersonalBalancesHandler : IQueryHandler<GetPersonalBalancesCommand, Result<GetPersonalBalancesResultData>>
{
    private readonly IBalanceRepository _balanceRepository;

    public GetPersonalBalancesHandler(IBalanceRepository balanceRepository)
    {
        _balanceRepository = balanceRepository;
    }

    public async ValueTask<Result<GetPersonalBalancesResultData>> Handle(GetPersonalBalancesCommand command, CancellationToken ct)
    {
        var accounts = await _balanceRepository.GetUserbalanceBatchAsync(command.UserId, ct);

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
