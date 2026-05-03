using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories;

public interface ISettlementRepository
{
    Task AddAsync(Settlement settlement);
}
