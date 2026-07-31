using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SplitSpace.Auth.Domain.Services;
using SplitSpace.Auth.Logic.Options;
using SplitSpace.Auth.Logic.Services;

namespace SplitSpace.Auth.Logic;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));

        services.AddScoped<IAccessTokenService, AccessTokenService>();
        services.AddScoped<PasswordHasher>();

        return services;
    }
}
