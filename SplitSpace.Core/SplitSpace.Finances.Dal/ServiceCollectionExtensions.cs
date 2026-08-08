using Dapper;
using Microsoft.Extensions.DependencyInjection;
using SplitSpace.Finances.Dal.ClientFacades;
using SplitSpace.Finances.Dal.ClientFacades.Implementations;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Repositories;
using SplitSpace.Finances.Dal.Database.Repositories.Implementations;
using SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;
using SplitSpace.Finances.Dal.Database.Transactions;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.SettlementAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Domain;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Finances.Dal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddSingleton<IFinanceDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<ITransactionProvider, TransactionProvider>();

        services.AddScoped<ICategoryDomainRepository, CategoryDomainRepository>();
        services.AddScoped<IBalanceDomainRepository, BalanceDomainRepository>();
        services.AddScoped<IExpenseDomainRepository, ExpenseDomainRepository>();
        services.AddScoped<ISettlementDomainRepository, SettlementDomainRepository>();
        services.AddScoped<IDebtDomainRepository, DebtDomainRepository>();
        services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
        services.AddScoped<IBalanceReadRepository, BalanceReadRepository>();
        services.AddScoped<IDebtReadRepository, DebtReadRepository>();
        services.AddScoped<IExpenseReadRepository, ExpenseReadRepository>();

        return services;
    }

    public static IServiceCollection AddClientFacades(
        this IServiceCollection services)
    {
        services.AddScoped<ISpacesServiceClientFacade, SpacesServiceClientFacade>();

        return services;
    }
}
