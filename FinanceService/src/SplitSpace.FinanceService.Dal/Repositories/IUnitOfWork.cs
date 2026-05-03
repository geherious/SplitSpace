namespace SplitSpace.FinanceService.Dal.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}