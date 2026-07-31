namespace SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;

public interface IDebtDomainRepository
{
    Task SaveAsync(Debt debt, CancellationToken cancellationToken);
    
    Task<Debt> GetOrCreate(Debt debt, CancellationToken cancellationToken);
}
