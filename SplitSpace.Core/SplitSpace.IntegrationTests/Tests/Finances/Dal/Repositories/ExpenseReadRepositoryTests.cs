using AutoFixture;
using FluentAssertions;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate.SplitPolicies;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.IntegrationTests.Fixtures;
using SplitSpace.IntegrationTests.Helpers.Common;
using SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;
using Xunit;

namespace SplitSpace.IntegrationTests.Tests.Finances.Dal.Repositories;

public sealed class ExpenseReadRepositoryTests : TestBase
{
    private readonly ExpenseTestRepository _sut;
    private readonly CategoryTestRepository _categories;
    private readonly BalanceTestRepository _balances;
    private readonly Fixture _fixture = new();

    public ExpenseReadRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<ExpenseTestRepository>();
        _categories = GetRequiredService<CategoryTestRepository>();
        _balances = GetRequiredService<BalanceTestRepository>();
    }

    private async Task<(SpaceId SpaceId, CategoryId CategoryId, BalanceId BalanceId)> SeedAsync(
        string spaceBalanceName = "Trip")
    {
        var spaceId = SpaceId.New();
        var createdBy = UserId.New();

        var categoryResult = Category.Create(spaceId, "Food", parent: null, limit: null);
        var category = categoryResult.ResultValue;
        category.Should().NotBeNull();
        await _categories.SaveAsync(category, CancellationToken.None);

        var balance = Balance.CreateSpace(spaceBalanceName, spaceId, createdBy);
        await _balances.SaveAsync(balance, CancellationToken.None);

        return (spaceId, category.Id, balance.Id);
    }

    private async Task<Expense> CreateExpenseAsync(
        SpaceId spaceId,
        UserId createdBy,
        CategoryId categoryId,
        BalanceId balanceId,
        decimal amount,
        string description,
        DateTimeOffset createdAt,
        params UserId[] participants)
    {
        var policy = new EqualSplitPolicy(new ExpenseSplitMethod.EqualSplit
        {
            Participants = participants.Select(p => new ExpenseSplitMethod.EqualSplit.SplitParticipant(p)).ToArray()
        });

        var result = Expense.Create(
            spaceId,
            createdBy,
            categoryId,
            balanceId,
            new Money(amount),
            description,
            createdAt,
            policy);

        result.IsSuccess.Should().BeTrue();
        var expense = result.ResultValue;
        expense.Should().NotBeNull();
        await _sut.SaveAsync(expense, CancellationToken.None);
        return expense;
    }

    [Fact]
    public async Task GetBatchAsync_ShouldReturnExpensesWithSplitsAndBalance()
    {
        // Arrange
        var (spaceId, categoryId, balanceId) = await SeedAsync();
        var createdBy = UserId.New();
        var participant = UserId.New();

        var expense = await CreateExpenseAsync(
            spaceId,
            createdBy,
            categoryId,
            balanceId,
            100m,
            "Dinner",
            _fixture.Create<DateTimeOffset>().ToUniversalTime(),
            createdBy,
            participant);

        // Act
        var result = await _sut.GetBatchAsync(spaceId.Value, CancellationToken.None);

        // Assert
        var persisted = result.Should().ContainSingle().Subject;
        persisted.ExpenseEntity.Id.Should().Be(expense.Id.Value);
        persisted.ExpenseEntity.Amount.Should().Be(100m);
        persisted.ExpenseEntity.Description.Should().Be("Dinner");
        persisted.Splits.Should().HaveCount(2);
        persisted.BalanceEntity.Id.Should().Be(balanceId.Value);
    }

    [Fact]
    public async Task GetGroupedByCategory_ShouldSumAmountsPerCategory()
    {
        // Arrange
        var (spaceId, categoryId, balanceId) = await SeedAsync();
        var createdBy = UserId.New();

        var from = _fixture.Create<DateTimeOffset>().ToUniversalTime();
        var to = from.AddDays(30);

        await CreateExpenseAsync(spaceId, createdBy, categoryId, balanceId, 60m, "Lunch", from.AddDays(1), createdBy);
        await CreateExpenseAsync(spaceId, createdBy, categoryId, balanceId, 40m, "Dinner", from.AddDays(2), createdBy);

        // Act
        var result = await _sut.GetGroupedByCategory(spaceId.Value, from, to, CancellationToken.None);

        // Assert
        var persisted = result.Should().ContainSingle().Subject;
        persisted.CategoryId.Should().Be(categoryId.Value);
        persisted.Amount.Should().Be(100m);
    }
}
