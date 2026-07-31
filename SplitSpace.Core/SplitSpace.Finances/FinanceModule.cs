using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SplitSpace.Finances.Dal;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Services;
using SplitSpace.SharedKernel.Database;

namespace SplitSpace.Finances;

public static class FinancesModule
{
    public static IServiceCollection Add(this IServiceCollection services)
    {
        services.AddRepositories();
        services.AddClientFacades();

        return services;
    }

    public static IEndpointRouteBuilder MapGrpc(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<FinanceServiceGrpc>();
        endpoints.MapGrpcService<SearchServiceGrpc>();

        return endpoints;
    }

    public static async Task RunMigrations(IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var connectionFactory = scope.ServiceProvider.GetRequiredService<IFinanceDbConnectionFactory>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseMigrator>>();
        var migrator = new DatabaseMigrator(
            typeof(DbConnectionFactory).Assembly,
            connectionFactory,
            logger);
        await migrator.MigrateAsync(ct);
    }
}
