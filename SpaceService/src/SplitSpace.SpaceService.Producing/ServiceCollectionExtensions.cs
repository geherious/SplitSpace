using Microsoft.Extensions.DependencyInjection;
using SplitSpace.SpaceService.Producing.Producers;
using SplitSpace.SpaceService.Producing.Producers.Implementations;

namespace SplitSpace.SpaceService.Producing;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProducers(this IServiceCollection services)
    {
        services.AddSingleton<ISpaceEventsProducer, SpaceEventsProducer>();

        return services;
    }
}
