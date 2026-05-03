using Microsoft.Extensions.DependencyInjection;
using SplitSpace.FinanceService.Consuming.Consumers;
using SplitSpace.FinanceService.Logic.Services;
using SplitSpace.FinanceService.Logic.Services.Implementations;

namespace SplitSpace.FinanceService.Consuming;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddHostedService<SpaceEventsConsumer>();
        
        return services;
    }
}
