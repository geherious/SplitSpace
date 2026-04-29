using Microsoft.Extensions.DependencyInjection;
using SplitSpace.SpaceService.Logic.Services;
using SplitSpace.SpaceService.Logic.Services.Implementations;

namespace SplitSpace.SpaceService.Logic;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddScoped<ISpaceService, Services.Implementations.SpaceService>();
        services.AddScoped<IInvitationService, InvitationService>();

        return services;
    }
}
