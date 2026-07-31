using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SplitSpace.Auth.Dal;
using SplitSpace.Auth.Dal.Database.Connections;
using SplitSpace.Auth.Logic;
using SplitSpace.Auth.Services;
using SplitSpace.SharedKernel.Database;

namespace SplitSpace.Auth;

public static class AuthModule
{
    public static IServiceCollection Add(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRepositories();
        services.AddAuthServices(configuration);

        return services;
    }

    public static IEndpointRouteBuilder MapGrpc(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<AuthServiceGrpc>();
        endpoints.MapGrpcService<UserServiceGrpc>();

        return endpoints;
    }

    public static async Task RunMigrations(IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var connectionFactory = scope.ServiceProvider.GetRequiredService<IAuthDbConnectionFactory>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseMigrator>>();
        var migrator = new DatabaseMigrator(
            typeof(DbConnectionFactory).Assembly,
            connectionFactory,
            logger);
        await migrator.MigrateAsync(ct);
    }
}
