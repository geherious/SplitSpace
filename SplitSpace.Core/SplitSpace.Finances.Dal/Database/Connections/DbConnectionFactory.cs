using Npgsql;

namespace SplitSpace.Finances.Dal.Database.Connections;

public class DbConnectionFactory : IFinanceDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory()
    {
        _connectionString = Environment.GetEnvironmentVariable("FINANCE_SERVICE_DB_CONNECTION_STRING")
            ?? "Host=localhost;Database=finance_db;Username=postgres;Password=postgres";
    }

    public async Task<NpgsqlConnection> CreateAsync(CancellationToken ct = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        return connection;
    }
}
