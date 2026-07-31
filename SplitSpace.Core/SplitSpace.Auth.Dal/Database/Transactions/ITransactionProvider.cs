namespace SplitSpace.Auth.Dal.Database.Transactions;

public interface ITransactionProvider
{
    Task<ITransactionContext> BeginAsync(CancellationToken ct);
}