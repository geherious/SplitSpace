using Xunit;

namespace SplitSpace.IntegrationTests.Fixtures;

[CollectionDefinition(Name)]
public sealed class TestCollection :
    ICollectionFixture<PostgresFixture>,
    ICollectionFixture<WebApplicationFixture>
{
    public const string Name = "Database";
}
