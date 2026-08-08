using Microsoft.Extensions.Configuration;
using Npgsql;
using SplitSpace.SharedKernel.Database.Connections;

namespace SplitSpace.Spaces.Dal.Database.Connections;

public class DbConnectionFactory : ISpaceDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SpaceServiceDb")
            ?? throw new InvalidOperationException("Connection string 'SpaceServiceDb' is not configured.");
    }

    public async Task<NpgsqlConnection> CreateAsync(CancellationToken ct = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        return connection;
    }
}
