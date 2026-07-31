using Dapper;
using Microsoft.Extensions.DependencyInjection;
using SplitSpace.Finances.Dal.ClientFacades;
using SplitSpace.Finances.Dal.ClientFacades.Implementations;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Repositories;
using SplitSpace.Finances.Dal.Database.Repositories.Implementations;
using SplitSpace.Finances.Dal.Database.Transactions;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;

namespace SplitSpace.Finances.Dal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(new BalanceOwnerTypeHandler());

        services.AddSingleton<IFinanceDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<ITransactionProvider, TransactionProvider>();

        services.AddScoped<IBalanceRepository, BalanceRepository>();
        services.AddScoped<ICategoryDomainRepository, CategoryDomainRepository>();
        services.AddScoped<IDebtRepository, DebtRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IExpenseSplitRepository, ExpenseSplitRepository>();
        services.AddScoped<ISettlementRepository, SettlementRepository>();
        services.AddScoped<IBalanceDomainRepository, BalanceDomainRepository>();
        services.AddScoped<IDebtDomainRepository, DebtDomainRepository>();
        services.AddScoped<IExpenseDomainRepository, ExpenseDomainRepository>();

        return services;
    }

    public static IServiceCollection AddClientFacades(
        this IServiceCollection services)
    {
        services.AddScoped<ISpacesServiceClientFacade, SpacesServiceClientFacade>();

        return services;
    }
}
