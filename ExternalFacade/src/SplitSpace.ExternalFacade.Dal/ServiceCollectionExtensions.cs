using Microsoft.Extensions.DependencyInjection;
using SplitSpace.ExternalFacade.Dal.ClientFacades;
using SplitSpace.ExternalFacade.Dal.ClientFacades.Implementations;
using SplitSpace.FinanceService.Api.SearchService;
using SplitSpace.SpaceService.Api;

namespace SplitSpace.ExternalFacade.Dal;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClientFacades(this IServiceCollection services)
    {
        services.AddGrpcClient<AuthService.Api.AuthService.AuthService.AuthServiceClient>(options =>
        {
            options.Address = new Uri("http://auth-service:5002");
        });
        
        services.AddGrpcClient<SpaceService.Api.SpaceService.SpaceServiceClient>(options =>
        {
            options.Address = new Uri("http://space-service:5002");
        });
        
        services.AddGrpcClient<InvitationService.InvitationServiceClient>(options =>
        {
            options.Address = new Uri("http://space-service:5002");
        });

        services.AddGrpcClient<FinanceService.Api.FinanceService.FinanceService.FinanceServiceClient>(options =>
        {
            options.Address = new Uri("http://finance-service:5002");
        });
        
        services.AddGrpcClient<SearchService.SearchServiceClient>(options =>
        {
            options.Address = new Uri("http://finance-service:5002");
        });

        services.AddScoped<IAuthServiceClientFacade, AuthServiceClientFacade>();
        services.AddScoped<IInvitationServiceClientFacade, InvitationServiceClientFacade>();
        services.AddScoped<ISpaceServiceClientFacade, SpaceServiceClientFacade>();
        services.AddScoped<IFinanceServiceClientFacade, FinanceServiceClientFacade>();
        services.AddScoped<ISearchServiceClientFacades, SearchServiceClientFacades>();

        return services;
    }
}