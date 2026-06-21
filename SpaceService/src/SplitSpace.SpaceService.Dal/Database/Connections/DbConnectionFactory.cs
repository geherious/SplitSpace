using Npgsql;

namespace SplitSpace.SpaceService.Dal.Database.Connections;

public class DbConnectionFactory : IDbConnectionFactory
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
