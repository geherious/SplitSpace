using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;

public interface IBalanceDomainRepository
{
    Task SaveAsync(Balance balance, CancellationToken cancellationToken);
    
    Task<Balance?> GetAsync(BalanceId balanceId, CancellationToken cancellationToken);
}
