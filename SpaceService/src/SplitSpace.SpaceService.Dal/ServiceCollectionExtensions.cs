using Microsoft.Extensions.DependencyInjection;
using SplitSpace.AuthService.Api.UserService;
using SplitSpace.SpaceService.Dal.ClientFacades;
using SplitSpace.SpaceService.Dal.ClientFacades.Implementations;
using SplitSpace.SpaceService.Dal.Database;
using SplitSpace.SpaceService.Dal.Database.Connections;
using SplitSpace.SpaceService.Dal.Database.Repositories.Implementations;
using SplitSpace.SpaceService.Dal.Database.Transactions;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Invitation;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

namespace SplitSpace.SpaceService.Dal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        services.AddScoped<DatabaseMigrator>();
        services.AddScoped<ITransactionProvider, TransactionProvider>();
        services.AddScoped<ISpaceDomainRepository, SpaceDomainRepository>();
        services.AddScoped<IInvitationDomainRepository, InvitationDomainRepository>();

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
