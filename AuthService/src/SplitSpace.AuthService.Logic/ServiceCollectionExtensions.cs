using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SplitSpace.AuthService.Logic.Options;
using SplitSpace.AuthService.Logic.Services;
using SplitSpace.AuthService.Logic.Services.Implementations;

namespace SplitSpace.AuthService.Logic;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAuthService, Services.Implementations.AuthService>();
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<ITokenService, TokenService>();
        serviceCollection.AddScoped<PasswordHasher>();
    }

    public static void AddServiceOptions(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<JwtOptions>(
            configuration.GetSection("JwtOptions"));
    }
}