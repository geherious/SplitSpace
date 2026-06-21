using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SplitSpace.AuthService.Api.UserService;
using SplitSpace.SpaceService.Dal.ClientFacades;
using SplitSpace.SpaceService.Dal.ClientFacades.Implementations;
using SplitSpace.SpaceService.Dal.Repositories;
using SplitSpace.SpaceService.Dal.Repositories.Implementations;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

namespace SplitSpace.SpaceService.Dal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddDbContext<SpaceServiceDbContext>(options =>
        {
            var connectionString = Environment.GetEnvironmentVariable("SPACE_SERVICE_DB_CONNECTION_STRING")
                                   ?? "Host=localhost;Database=space_db;Username=postgres;Password=postgres";

            options.UseNpgsql(connectionString);
        });
        
        services.AddScoped<DatabaseMigrator>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ISpaceDomainRepository, SpaceDomainRepository>();

        return services;
    }

    public static IServiceCollection AddClientFacades(this IServiceCollection services)
    {
        services.AddGrpcClient<UserService.UserServiceClient>(options =>
        {
            options.Address = new Uri("http://auth-service:5002");
        });
            
        services.AddScoped<IAuthServiceClientFacade, AuthServiceClientFacade>();
        
        return services;
    }
}
