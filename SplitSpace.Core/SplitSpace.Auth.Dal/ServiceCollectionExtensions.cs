using Microsoft.Extensions.DependencyInjection;
using SplitSpace.Auth.Dal.Database.Connections;
using SplitSpace.Auth.Dal.Database.Repositories.Implementations;
using SplitSpace.Auth.Dal.Database.Transactions;
using SplitSpace.Auth.Domain.Models.Aggregates.RefreshTokenAggregate;
using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;

namespace SplitSpace.Auth.Dal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IAuthDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<ITransactionProvider, TransactionProvider>();
        services.AddScoped<IUserDomainRepository, UserDomainRepository>();
        services.AddScoped<IRefreshTokenDomainRepository, RefreshTokenDomainRepository>();

        return services;
    }
}
