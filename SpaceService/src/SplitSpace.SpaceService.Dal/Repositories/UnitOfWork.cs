using Microsoft.EntityFrameworkCore.Storage;

namespace SplitSpace.SpaceService.Dal.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly SpaceServiceDbContext _context;

    public UnitOfWork(SpaceServiceDbContext context) => _context = context;

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken ct)
    {
        var tx = await _context.Database.BeginTransactionAsync(ct);
        return new EfTransactionWrapper(tx);
    }

    private class EfTransactionWrapper : IUnitOfWorkTransaction
    {
        private readonly IDbContextTransaction _tx;
        private bool _completed;

        public EfTransactionWrapper(IDbContextTransaction tx) => _tx = tx;

        public async Task CommitAsync(CancellationToken ct)
        {
            await _tx.CommitAsync(ct);
            _completed = true;
        }

        public async Task RollbackAsync(CancellationToken ct)
        {
            await _tx.RollbackAsync(ct);
            _completed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_completed)
                await _tx.RollbackAsync(); // safety net if caller forgets to commit
            await _tx.DisposeAsync();
        }
    }
}
