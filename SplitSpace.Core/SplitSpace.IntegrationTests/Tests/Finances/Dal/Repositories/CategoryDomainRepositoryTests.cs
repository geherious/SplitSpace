using FluentAssertions;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.IntegrationTests.Fixtures;
using SplitSpace.IntegrationTests.Helpers.Common;
using SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;
using Xunit;

namespace SplitSpace.IntegrationTests.Tests.Finances.Dal.Repositories;

public sealed class CategoryDomainRepositoryTests : TestBase
{
    private readonly CategoryTestRepository _sut;

    public CategoryDomainRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<CategoryTestRepository>();
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistCategory()
    {
        // Arrange
        var spaceId = SpaceId.New();
        var result = Category.Create(spaceId, "Food", parent: null, limit: 500m);
        result.IsSuccess.Should().BeTrue();
        var category = result.ResultValue;
        category.Should().NotBeNull();

        // Act
        await _sut.SaveAsync(category, CancellationToken.None);
        var loaded = await _sut.GetAsync(category.Id, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Id.Should().Be(category.Id);
        loaded.SpaceId.Should().Be(spaceId);
        loaded.Name.Should().Be("Food");
        loaded.Limit.Should().Be(500m);
        loaded.ParentId.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WhenCategoryDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var categoryId = CategoryId.New();

        // Act
        var loaded = await _sut.GetAsync(categoryId, CancellationToken.None);

        // Assert
        loaded.Should().BeNull();
    }
}
