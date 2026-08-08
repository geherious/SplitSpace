using Microsoft.Extensions.DependencyInjection;
using SplitSpace.IntegrationTests.Fixtures;
using Xunit;

namespace SplitSpace.IntegrationTests.Helpers.Common;

[Collection(TestCollection.Name)]
public abstract class TestBase : IAsyncLifetime
{
    private readonly AsyncServiceScope _scope;

    protected IServiceProvider Services { get; }

    protected TestBase(WebApplicationFixture fixture)
    {
        _scope = fixture.Services.CreateAsyncScope();
        Services = _scope.ServiceProvider;
    }

    protected T GetRequiredService<T>() where T : notnull
    {
        return Services.GetRequiredService<T>();
    }

    public ValueTask DisposeAsync()
    {
        return _scope.DisposeAsync();
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
