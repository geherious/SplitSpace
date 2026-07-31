using Npgsql;
using SplitSpace.SharedKernel.Database.Connections;

namespace SplitSpace.Spaces.Dal.Database.Connections;

public class DbConnectionFactory : ISpaceDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory()
    {
        _connectionString = Environment.GetEnvironmentVariable("SPACE_SERVICE_DB_CONNECTION_STRING")
            ?? "Host=localhost;Database=space_db;Username=postgres;Password=postgres";
    }

    public async Task<NpgsqlConnection> CreateAsync(CancellationToken ct = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        return connection;
    }
}
