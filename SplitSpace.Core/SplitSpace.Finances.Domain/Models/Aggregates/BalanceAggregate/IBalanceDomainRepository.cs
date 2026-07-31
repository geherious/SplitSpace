using SplitSpace.Finances.Domain.Models.Ids;

namespace SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;

public interface IBalanceDomainRepository
{
    Task SaveAsync(Balance balance, CancellationToken cancellationToken);
    
    Task<Balance?> GetAsync(BalanceId balanceId, CancellationToken cancellationToken);
}
