using Dapper;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace SplitSpace.IntegrationTests.Fixtures;

public sealed class PostgresFixture : IAsyncLifetime
{
    public const string FinanceDbName = "finance_db";
    public const string AuthDbName = "auth_db";
    public const string SpaceDbName = "space_db";

    private const string Image = "postgres:16-alpine";
    private const int HostPort = 5499;

    private PostgreSqlContainer _postgres = null!;

    public string FinanceConnectionString => ConnectionStringFor(FinanceDbName);
    public string AuthConnectionString => ConnectionStringFor(AuthDbName);
    public string SpaceConnectionString => ConnectionStringFor(SpaceDbName);

    public async ValueTask InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder(Image)
            .WithPortBinding(HostPort, 5432)
            .Build();

        await _postgres.StartAsync();

        await CreateDatabasesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_postgres is not null)
        {
            await _postgres.DisposeAsync();
        }
    }

    private async Task CreateDatabasesAsync()
    {
        await using var connection = new NpgsqlConnection(ConnectionStringFor("postgres"));
        await connection.OpenAsync();

        foreach (var databaseName in new[] { FinanceDbName, AuthDbName, SpaceDbName })
        {
            var exists = await connection.ExecuteScalarAsync<bool>(
                "SELECT EXISTS (SELECT 1 FROM pg_database WHERE datname = @name)",
                new { name = databaseName });

            if (!exists)
            {
                await connection.ExecuteAsync($"CREATE DATABASE {databaseName}");
            }
        }
    }

    private string ConnectionStringFor(string databaseName)
    {
        var builder = new NpgsqlConnectionStringBuilder(_postgres.GetConnectionString())
        {
            Database = databaseName
        };

        return builder.ConnectionString;
    }
}
