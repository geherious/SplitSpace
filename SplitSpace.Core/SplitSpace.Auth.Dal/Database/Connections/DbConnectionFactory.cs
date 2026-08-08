using Microsoft.Extensions.Configuration;
using Npgsql;

namespace SplitSpace.Auth.Dal.Database.Connections;

public class DbConnectionFactory : IAuthDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AuthServiceDb")
            ?? throw new InvalidOperationException("Connection string 'AuthServiceDb' is not configured.");
    }

    public async Task<NpgsqlConnection> CreateAsync(CancellationToken ct = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        return connection;
    }
}
