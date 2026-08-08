using FluentAssertions;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.IntegrationTests.Fixtures;
using SplitSpace.IntegrationTests.Helpers.Common;
using SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;
using Xunit;

namespace SplitSpace.IntegrationTests.Tests.Finances.Dal.Repositories;

public sealed class CategoryReadRepositoryTests : TestBase
{
    private readonly CategoryTestRepository _sut;

    public CategoryReadRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<CategoryTestRepository>();
    }

    [Fact]
    public async Task GetBatchAsync_ShouldReturnCategoriesForSpace()
    {
        // Arrange
        var spaceId = SpaceId.New();
        var otherSpaceId = SpaceId.New();

        var food = Category.Create(spaceId, "Food", parent: null, limit: 1000m).ResultValue;
        food.Should().NotBeNull();
        var transport = Category.Create(spaceId, "Transport", parent: null, limit: null).ResultValue;
        transport.Should().NotBeNull();
        var other = Category.Create(otherSpaceId, "Other", parent: null, limit: null).ResultValue;
        other.Should().NotBeNull();

        await _sut.SaveAsync(food, CancellationToken.None);
        await _sut.SaveAsync(transport, CancellationToken.None);
        await _sut.SaveAsync(other, CancellationToken.None);

        // Act
        var result = await _sut.GetBatchAsync(spaceId, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Id == food.Id && c.Name == "Food");
        result.Should().Contain(c => c.Id == transport.Id && c.Name == "Transport");
        result.Should().NotContain(c => c.Id == other.Id);
    }

    [Fact]
    public async Task GetBatchAsync_WhenNoCategories_ShouldReturnEmpty()
    {
        // Arrange
        var spaceId = SpaceId.New();

        // Act
        var result = await _sut.GetBatchAsync(spaceId, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
