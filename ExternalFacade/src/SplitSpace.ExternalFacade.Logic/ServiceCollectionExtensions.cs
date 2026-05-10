using Microsoft.Extensions.DependencyInjection;
using SplitSpace.ExternalFacade.Logic.Services;
using SplitSpace.ExternalFacade.Logic.Services.Implementations;

namespace SplitSpace.ExternalFacade.Logic;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        
        return services;
    }
}
