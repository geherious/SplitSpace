using System.Data;
using SplitSpace.Spaces.Domain.Models.Aggregates.InvitationAggregate;
using SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;

namespace SplitSpace.Spaces.Dal.Database.Transactions;

public interface ITransactionContext : IAsyncDisposable
{
    RepositoryRegistry Repositories { get; }

    IDbTransaction Transaction { get; }
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);
    
    public record RepositoryRegistry
    {
        public required ISpaceDomainRepository SpaceDomainRepository  { get; init; }
        
        public required IInvitationDomainRepository InvitationDomainRepository  { get; init; }
    }
}
