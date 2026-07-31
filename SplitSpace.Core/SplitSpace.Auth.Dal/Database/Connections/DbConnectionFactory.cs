using Npgsql;

namespace SplitSpace.Auth.Dal.Database.Connections;

public class DbConnectionFactory : IAuthDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory()
    {
        _connectionString = Environment.GetEnvironmentVariable("AUTH_SERVICE_DB_CONNECTION_STRING")
            ?? "Host=localhost;Database=auth_db;Username=postgres;Password=postgres";
    }

    public async Task<NpgsqlConnection> CreateAsync(CancellationToken ct = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        return connection;
    }
}
