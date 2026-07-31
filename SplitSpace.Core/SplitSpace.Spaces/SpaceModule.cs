using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SplitSpace.Spaces.Dal;
using SplitSpace.Spaces.Dal.Database.Connections;
using SplitSpace.Spaces.Services;
using SplitSpace.SharedKernel.Database;

namespace SplitSpace.Spaces;

public static class SpacesModule
{
    public static IServiceCollection Add(this IServiceCollection services)
    {
        services.AddRepositories();
        services.AddClientFacades();

        return services;
    }

    public static IEndpointRouteBuilder MapGrpc(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<SpaceServiceGrpc>();
        endpoints.MapGrpcService<InvitationServiceGrpc>();

        return endpoints;
    }

    public static async Task RunMigrations(IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var connectionFactory = scope.ServiceProvider.GetRequiredService<ISpaceDbConnectionFactory>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseMigrator>>();
        var migrator = new DatabaseMigrator(
            typeof(DbConnectionFactory).Assembly,
            connectionFactory,
            logger);
        await migrator.MigrateAsync(ct);
    }
}
