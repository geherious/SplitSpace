using FluentAssertions;
using SplitSpace.Auth.Domain.Models.Aggregates.RefreshTokenAggregate;
using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;
using SplitSpace.Auth.Domain.Models.ValueObjects;
using SplitSpace.Auth.Logic.Services;
using SplitSpace.IntegrationTests.Fixtures;
using SplitSpace.IntegrationTests.Helpers.Auth.TestRepositories;
using SplitSpace.IntegrationTests.Helpers.Common;
using Xunit;

namespace SplitSpace.IntegrationTests.Tests.Auth.Dal.Repositories;

public sealed class RefreshTokenDomainRepositoryTests : TestBase
{
    private readonly RefreshTokenTestRepository _sut;
    private readonly UserTestRepository _userRepository;
    private readonly PasswordHasher _hasher;

    public RefreshTokenDomainRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<RefreshTokenTestRepository>();
        _userRepository = GetRequiredService<UserTestRepository>();
        _hasher = GetRequiredService<PasswordHasher>();
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistRefreshToken()
    {
        // Arrange
        var user = await CreateUserAsync();
        var createdAt = DateTimeOffset.UtcNow;
        var (refreshToken, rawToken) = RefreshToken.Create(user.Id, createdAt);

        // Act
        await _sut.SaveAsync(refreshToken, CancellationToken.None);
        var loaded = await _sut.GetAsync(rawToken, CancellationToken.None);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Id.Should().Be(refreshToken.Id);
        loaded.UserId.Should().Be(user.Id);
        loaded.Token.Value.Should().Be(RefreshTokenHash.FromRaw(rawToken).Value);
        loaded.ExpiresAt.Should().BeCloseTo(createdAt.Add(TimeSpan.FromDays(15)), TimeSpan.FromMilliseconds(1));
        loaded.RevokedAt.Should().BeNull();
        loaded.CreatedAt.Should().BeCloseTo(createdAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task GetAsync_WhenTokenDoesNotExist_ShouldReturnNull()
    {
        // Act
        var loaded = await _sut.GetAsync("does-not-exist", CancellationToken.None);

        // Assert
        loaded.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldNotReturnRevokedToken()
    {
        // Arrange
        var user = await CreateUserAsync();
        var createdAt = DateTimeOffset.UtcNow;
        var (refreshToken, rawToken) = RefreshToken.Create(user.Id, createdAt);
        await _sut.SaveAsync(refreshToken, CancellationToken.None);

        var revokedAt = createdAt.AddDays(1);
        var revoked = RefreshToken.Rehydrate(
            refreshToken.Id,
            user.Id,
            RefreshTokenHash.FromRaw(rawToken),
            createdAt.Add(TimeSpan.FromDays(15)),
            revokedAt: null,
            createdAt);
        revoked.Revoke(revokedAt);

        // Act
        await _sut.SaveAsync(revoked, CancellationToken.None);
        var loaded = await _sut.GetAsync(rawToken, CancellationToken.None);

        // Assert
        loaded.Should().BeNull();
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistMultipleTokensInSingleTransaction()
    {
        // Arrange
        var user = await CreateUserAsync();
        var createdAt = DateTimeOffset.UtcNow;
        var (first, firstRaw) = RefreshToken.Create(user.Id, createdAt);
        var (second, secondRaw) = RefreshToken.Create(user.Id, createdAt);

        // Act
        await _sut.SaveAsync([first, second], CancellationToken.None);
        var firstLoaded = await _sut.GetAsync(firstRaw, CancellationToken.None);
        var secondLoaded = await _sut.GetAsync(secondRaw, CancellationToken.None);

        // Assert
        firstLoaded.Should().NotBeNull();
        secondLoaded.Should().NotBeNull();
        firstLoaded.Token.Value.Should().Be(RefreshTokenHash.FromRaw(firstRaw).Value);
        secondLoaded.Token.Value.Should().Be(RefreshTokenHash.FromRaw(secondRaw).Value);
    }

    private async Task<User> CreateUserAsync()
    {
        var createdAt = DateTimeOffset.UtcNow;
        var result = User.Create(
            $"{Guid.NewGuid():N}@example.com",
            "Password123!",
            _hasher,
            createdAt);
        result.IsSuccess.Should().BeTrue();
        var user = result.ResultValue;
        user.Should().NotBeNull();
        await _userRepository.SaveAsync(user, CancellationToken.None);
        return user;
    }
}
