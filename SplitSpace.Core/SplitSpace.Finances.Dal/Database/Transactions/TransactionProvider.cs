using System.Data;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Repositories.Implementations;

namespace SplitSpace.Finances.Dal.Database.Transactions;

public class TransactionProvider : ITransactionProvider
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;

    public TransactionProvider(IFinanceDbConnectionFactory connectionFactory)
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
        private readonly IFinanceDbConnectionFactory _connectionFactory;
        private readonly NpgsqlConnection _connection;
        private readonly NpgsqlTransaction _transaction;
        private bool _completed;

        public IDbTransaction Transaction => _transaction;

        public TransactionContext(
            IFinanceDbConnectionFactory connectionFactory,
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
                BalanceRepository = new BalanceRepository(_connectionFactory, _transaction),
                CategoryRepository = new CategoryDomainRepository(_connectionFactory, _transaction),
                DebtRepository = new DebtRepository(_connectionFactory, _transaction),
                ExpenseRepository = new ExpenseRepository(_connectionFactory, _transaction),
                ExpenseSplitRepository = new ExpenseSplitRepository(_connectionFactory, _transaction),
                SettlementRepository = new SettlementRepository(_connectionFactory, _transaction)
            };
    }
}
