using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SplitSpace.SpaceService.Dal;

public class DatabaseMigrator
{
    private readonly SpaceServiceDbContext _dbContext;
    private readonly ILogger<DatabaseMigrator> _logger;

    public DatabaseMigrator(
        SpaceServiceDbContext dbContext,
        ILogger<DatabaseMigrator> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting database migration...");

        try
        {
            await _dbContext.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
    }
}