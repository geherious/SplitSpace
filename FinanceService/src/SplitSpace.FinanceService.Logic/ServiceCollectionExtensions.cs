using Microsoft.Extensions.DependencyInjection;
using SplitSpace.FinanceService.Logic.Services;
using SplitSpace.FinanceService.Logic.Services.Implementations;

namespace SplitSpace.FinanceService.Logic;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IDebtService, DebtService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ISearchService, SearchService>();
        
        return services;
    }
}
