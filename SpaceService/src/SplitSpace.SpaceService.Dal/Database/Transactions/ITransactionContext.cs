using System.Data;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Invitation;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

namespace SplitSpace.SpaceService.Dal.Database.Transactions;

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
