using FluentAssertions;
using Npgsql;
using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;
using SplitSpace.Auth.Domain.Models.Ids;
using SplitSpace.Auth.Domain.Models.ValueObjects;
using SplitSpace.Auth.Logic.Services;
using SplitSpace.IntegrationTests.Fixtures;
using SplitSpace.IntegrationTests.Helpers.Auth.TestRepositories;
using SplitSpace.IntegrationTests.Helpers.Common;
using Xunit;

namespace SplitSpace.IntegrationTests.Tests.Auth.Dal.Repositories;

public sealed class UserDomainRepositoryTests : TestBase
{
    private readonly UserTestRepository _sut;
    private readonly PasswordHasher _hasher;

    public UserDomainRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<UserTestRepository>();
        _hasher = GetRequiredService<PasswordHasher>();
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistUser()
    {
        // Arrange
        var createdAt = DateTimeOffset.UtcNow;
        var result = User.Create("  Test.User@Example.COM ", "Password123!", _hasher, createdAt);
        result.IsSuccess.Should().BeTrue();
        var user = result.ResultValue;
        user.Should().NotBeNull();

        // Act
        await _sut.SaveAsync(user, CancellationToken.None);
        var loaded = await _sut.GetAsync(user.Id, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Id.Should().Be(user.Id);
        loaded.Email.Value.Should().Be("test.user@example.com");
        loaded.CreatedAt.Should().BeCloseTo(createdAt, TimeSpan.FromMilliseconds(1));
        loaded.LastLogin.Should().BeNull();
        _hasher.Verify("Password123!", loaded.PasswordHash.Value).Should().BeTrue();
    }

    [Fact]
    public async Task GetAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var userId = UserId.New();
        var emailResult = Email.Create("nobody@example.com");
        emailResult.IsSuccess.Should().BeTrue();

        // Act
        var byId = await _sut.GetAsync(userId, CancellationToken.None);
        var byEmail = await _sut.GetAsync(emailResult.ResultValue, CancellationToken.None);

        // Assert
        byId.Should().BeNull();
        byEmail.Should().BeNull();
    }

    [Fact]
    public async Task SaveAsync_WhenEmailAlreadyExists_ShouldThrow()
    {
        // Arrange
        var createdAt = DateTimeOffset.UtcNow;
        var first = User.Create("duplicate@example.com", "Password123!", _hasher, createdAt).ResultValue;
        first.Should().NotBeNull();
        await _sut.SaveAsync(first, CancellationToken.None);

        var second = User.Create("duplicate@example.com", "Password123!", _hasher, createdAt).ResultValue;
        second.Should().NotBeNull();

        // Act
        var act = () => _sut.SaveAsync(second, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<PostgresException>();
    }
}
