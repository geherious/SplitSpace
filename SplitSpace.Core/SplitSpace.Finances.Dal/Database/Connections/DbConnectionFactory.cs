using Microsoft.Extensions.Configuration;
using Npgsql;

namespace SplitSpace.Finances.Dal.Database.Connections;

public class DbConnectionFactory : IFinanceDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("FinanceServiceDb")
            ?? throw new InvalidOperationException("Connection string 'FinanceServiceDb' is not configured.");
    }

    public async Task<NpgsqlConnection> CreateAsync(CancellationToken ct = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        return connection;
    }
}
