using System.Reflection;
using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Extensions.Logging;
using SplitSpace.SharedKernel.Database.Connections;

namespace SplitSpace.SharedKernel.Database;

public partial class DatabaseMigrator
{
    private readonly Assembly _migrationsAssembly;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DatabaseMigrator> _logger;

    public DatabaseMigrator(
        Assembly migrationsAssembly,
        IDbConnectionFactory connectionFactory,
        ILogger<DatabaseMigrator> logger)
    {
        _migrationsAssembly = migrationsAssembly;
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting database migration...");

        await using var connection = await _connectionFactory.CreateAsync(ct);

        await EnsureGooseTableAsync(connection, ct);

        var appliedVersions = (await connection.QueryAsync<long>(
            "SELECT version_id FROM goose_db_version WHERE is_applied = true ORDER BY version_id", ct)).ToHashSet();

        var pending = GetPendingUpMigrations(appliedVersions);

        foreach (var migration in pending)
        {
            _logger.LogInformation("Applying migration: {Version} - {Description}", migration.Version, migration.Description);

            await using var tx = await connection.BeginTransactionAsync(ct);
            try
            {
                await connection.ExecuteAsync(migration.UpSql, transaction: tx);
                await connection.ExecuteAsync(
                    "INSERT INTO goose_db_version (version_id, is_applied) VALUES (@VersionId, true)",
                    new { VersionId = migration.Version }, tx);
                await tx.CommitAsync(ct);

                _logger.LogInformation("Applied migration: {Version}", migration.Version);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                _logger.LogError("Failed to apply migration: {Version}", migration.Version);
                throw;
            }
        }

        if (pending.Count == 0)
            _logger.LogInformation("Database is up to date");

        _logger.LogInformation("Database migration completed");
    }

    private static async Task EnsureGooseTableAsync(Npgsql.NpgsqlConnection connection, CancellationToken ct)
    {
        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS goose_db_version (
                id          SERIAL      PRIMARY KEY,
                version_id  BIGINT      NOT NULL,
                is_applied  BOOLEAN     NOT NULL,
                tstamp      TIMESTAMP   DEFAULT NOW()
            )
            """, ct);
    }

    private List<GooseMigration> GetPendingUpMigrations(HashSet<long> applied)
    {
        var assemblyName = _migrationsAssembly.GetName().Name!;

        var pending = _migrationsAssembly.GetManifestResourceNames()
            .Where(r => r.StartsWith($"{assemblyName}.")
                && r.Contains(".Migrations.")
                && r.EndsWith(".sql"))
            .Select(LoadMigration)
            .OrderBy(m => m.Version)
            .Where(m => !applied.Contains(m.Version))
            .ToList();

        return pending;
    }

    private GooseMigration LoadMigration(string resourceName)
    {
        using var stream = _migrationsAssembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

        var fileName = Path.GetFileNameWithoutExtension(resourceName);
        var versionMatch = VersionRegex().Match(fileName);
        var description = fileName[versionMatch.Length..].TrimStart('_');

        var upMatch = UpSectionRegex().Match(content);
        var downMatch = DownSectionRegex().Match(content);

        var upSql = upMatch.Success
            ? content[upMatch.Index..(downMatch.Success ? downMatch.Index : content.Length)]
                .Replace("-- +goose Up", "")
                .Trim()
            : "";

        return new GooseMigration(
            long.Parse(versionMatch.Value),
            description,
            upSql);
    }

    [GeneratedRegex(@"^\d+")]
    private static partial Regex VersionRegex();

    [GeneratedRegex("-- \\+goose Up")]
    private static partial Regex UpSectionRegex();

    [GeneratedRegex("-- \\+goose Down")]
    private static partial Regex DownSectionRegex();

    private sealed record GooseMigration(long Version, string Description, string UpSql);
}
