using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SplitSpace.IntegrationTests.Helpers.Auth.TestRepositories;
using SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;
using SplitSpace.IntegrationTests.Helpers.Spaces.TestRepositories;
using Xunit;

namespace SplitSpace.IntegrationTests.Fixtures;

public sealed class WebApplicationFixture : IAsyncLifetime
{
    private WebApplicationFactory<Program>? _factory;

    public IServiceProvider Services => (_factory ??= CreateFactory()).Services;

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }
    }

    private WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<BalanceTestRepository>();
                    services.AddScoped<CategoryTestRepository>();
                    services.AddScoped<DebtTestRepository>();
                    services.AddScoped<ExpenseTestRepository>();
                    services.AddScoped<SettlementTestRepository>();
                    services.AddScoped<UserTestRepository>();
                    services.AddScoped<RefreshTokenTestRepository>();
                    services.AddScoped<SpaceTestRepository>();
                    services.AddScoped<InvitationTestRepository>();
                });
            });
    }
}
