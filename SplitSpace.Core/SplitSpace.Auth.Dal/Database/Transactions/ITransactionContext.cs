using System.Data;
using SplitSpace.Auth.Domain.Models.Aggregates.RefreshTokenAggregate;
using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;

namespace SplitSpace.Auth.Dal.Database.Transactions;

public interface ITransactionContext : IAsyncDisposable
{
    RepositoryRegistry Repositories { get; }

    IDbTransaction Transaction { get; }
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);
    
    public record RepositoryRegistry
    {
        public required IUserDomainRepository UserDomainRepository  { get; init; }
        
        public required IRefreshTokenDomainRepository RefreshTokenDomainRepository  { get; init; }
    }
}
