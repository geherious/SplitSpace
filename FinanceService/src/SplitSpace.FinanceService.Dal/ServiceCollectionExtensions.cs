using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SplitSpace.FinanceService.Dal.Repositories;
using SplitSpace.FinanceService.Dal.Repositories.Implementations;
using SplitSpace.SpaceService.Dal.Repositories;
using SplitSpace.SpaceService.Dal.Repositories.Implementations;

namespace SplitSpace.FinanceService.Dal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddDbContext<FinanceServiceDbContext>(options =>
        {
            var connectionString = Environment.GetEnvironmentVariable("SPACE_SERVICE_DB_CONNECTION_STRING")
                                   ?? "Host=localhost;Database=space_db;Username=postgres;Password=postgres";

            options.UseNpgsql(connectionString);
        });
        
        services.AddScoped<DatabaseMigrator>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IDebtRepository, DebtRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IExpenseSplitRepository, ExpenseSplitRepository>();
        services.AddScoped<ISettlementRepository, SettlementRepository>();
        services.AddScoped<ISpaceMembershipRepository, SpaceMembershipRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

        return services;
    }
}
