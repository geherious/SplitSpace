using System.Data;
using Npgsql;
using SplitSpace.SpaceService.Dal.Database.Connections;
using SplitSpace.SpaceService.Dal.Database.Repositories.Implementations;

namespace SplitSpace.SpaceService.Dal.Database.Transactions;

public class TransactionProvider : ITransactionProvider
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TransactionProvider(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ITransactionContext> BeginAsync(CancellationToken ct)
    {
        var connection = await _connectionFactory.CreateAsync(ct);
        var transaction = await connection.BeginTransactionAsync(ct);
        return new TransactionContext(_connectionFactory, connection, transaction);
    }

    private class TransactionContext : ITransactionContext
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly NpgsqlConnection _connection;
        private readonly NpgsqlTransaction _transaction;
        private bool _completed;

        public IDbTransaction Transaction => _transaction;

        public TransactionContext(
            IDbConnectionFactory connectionFactory,
            NpgsqlConnection connection,
            NpgsqlTransaction transaction)
        {
            _connectionFactory = connectionFactory;
            _connection = connection;
            _transaction = transaction;
        }

        public async Task CommitAsync(CancellationToken ct)
        {
            await _transaction.CommitAsync(ct);
            _completed = true;
        }

        public async Task RollbackAsync(CancellationToken ct)
        {
            await _transaction.RollbackAsync(ct);
            _completed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (!_completed)
                await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            await _connection.DisposeAsync();
        }

        public ITransactionContext.RepositoryRegistry Repositories =>
            new()
            {
                SpaceDomainRepository = new SpaceDomainRepository(_connectionFactory, _transaction),
                InvitationDomainRepository = new InvitationDomainRepository(_connectionFactory, _transaction)
            };
    }
}
