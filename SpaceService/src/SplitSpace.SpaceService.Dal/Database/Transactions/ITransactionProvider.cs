namespace SplitSpace.SpaceService.Dal.Database.Transactions;

public interface ITransactionProvider
{
    Task<ITransactionContext> BeginAsync(CancellationToken ct);
}