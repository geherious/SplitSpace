using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SplitSpace.AuthService.Dal.Repositories;
using SplitSpace.AuthService.Dal.repositories.Implementations;

namespace SplitSpace.AuthService.Dal;

public static class ServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<AuthServiceDbContext>(options =>
        {
            var connectionString = Environment.GetEnvironmentVariable("AUTH_SERVICE_DB_CONNECTION_STRING")
                                   ?? "Host=localhost;Database=auth_db;Username=postgres;Password=postgres";

            options.UseNpgsql(connectionString);
        });

        serviceCollection.AddScoped<DatabaseMigrator>();

        serviceCollection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
    }
}