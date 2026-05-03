using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories;

public interface IDebtRepository
{
    Task AddOrUpdateAsync(IReadOnlyCollection<Debt> debts);
    
    Task <IReadOnlyCollection<Debt>> GetBatchAsync(Guid spaceId, Guid userId);
}
