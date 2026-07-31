using Microsoft.Extensions.DependencyInjection;
using SplitSpace.Spaces.Dal.ClientFacades;
using SplitSpace.Spaces.Dal.ClientFacades.Implementations;
using SplitSpace.Spaces.Dal.Database.Connections;
using SplitSpace.Spaces.Dal.Database.Repositories.Implementations;
using SplitSpace.Spaces.Dal.Database.Transactions;
using SplitSpace.Spaces.Domain.Models.Aggregates.InvitationAggregate;
using SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;

namespace SplitSpace.Spaces.Dal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ISpaceDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<ITransactionProvider, TransactionProvider>();
        services.AddScoped<ISpaceDomainRepository, SpaceDomainRepository>();
        services.AddScoped<IInvitationDomainRepository, InvitationDomainRepository>();

        return services;
    }

    public static IServiceCollection AddClientFacades(this IServiceCollection services)
    {
        services.AddScoped<IAuthServiceClientFacade, AuthServiceClientFacade>();

        return services;
    }
}
