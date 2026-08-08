using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.SettlementAggregate;

public interface ISettlementDomainRepository
{
    Task SaveAsync(Settlement settlement, CancellationToken cancellationToken);
}
