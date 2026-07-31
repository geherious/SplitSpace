using Npgsql;
using SplitSpace.Auth.Dal.Database.Connections;

namespace SplitSpace.Auth.Dal.Database.Repositories;

public abstract class BaseRepository
{
    protected readonly IAuthDbConnectionFactory _connectionFactory;
    protected readonly NpgsqlTransaction? _transaction;

    public BaseRepository(IAuthDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal BaseRepository(IAuthDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }
}
