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

public sealed class ExpenseDomainRepositoryTests : TestBase
{
    private readonly ExpenseTestRepository _sut;
    private readonly CategoryTestRepository _categories;
    private readonly BalanceTestRepository _balances;
    private readonly Fixture _fixture = new();

    public ExpenseDomainRepositoryTests(WebApplicationFixture fixture)
        : base(fixture)
    {
        _sut = GetRequiredService<ExpenseTestRepository>();
        _categories = GetRequiredService<CategoryTestRepository>();
        _balances = GetRequiredService<BalanceTestRepository>();
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistExpenseWithSplits()
    {
        // Arrange
        var spaceId = SpaceId.New();
        var createdBy = UserId.New();
        var participant = UserId.New();

        var categoryResult = Category.Create(spaceId, "Food", parent: null, limit: null);
        categoryResult.IsSuccess.Should().BeTrue();
        var category = categoryResult.ResultValue;
        category.Should().NotBeNull();
        await _categories.SaveAsync(category, CancellationToken.None);

        var balance = Balance.CreateSpace("Trip", spaceId, createdBy);
        await _balances.SaveAsync(balance, CancellationToken.None);

        var policy = new EqualSplitPolicy(new ExpenseSplitMethod.EqualSplit
        {
            Participants =
            [
                new ExpenseSplitMethod.EqualSplit.SplitParticipant(createdBy),
                new ExpenseSplitMethod.EqualSplit.SplitParticipant(participant)
            ]
        });

        var createdAt = _fixture.Create<DateTimeOffset>().ToUniversalTime();
        var expenseResult = Expense.Create(
            spaceId,
            createdBy,
            category.Id,
            balance.Id,
            new Money(100m),
            "Dinner",
            createdAt,
            policy);

        expenseResult.IsSuccess.Should().BeTrue();
        var expense = expenseResult.ResultValue;
        expense.Should().NotBeNull();
        expense.ExpenseSplits.Should().HaveCount(2);

        // Act
        await _sut.SaveAsync(expense, CancellationToken.None);
        var loaded = await _sut.GetBatchAsync(spaceId.Value, CancellationToken.None);

        // Assert
        var persisted = loaded.Should().ContainSingle().Subject;
        persisted.ExpenseEntity.Id.Should().Be(expense.Id.Value);
        persisted.ExpenseEntity.Amount.Should().Be(100m);
        persisted.ExpenseEntity.Description.Should().Be("Dinner");
        persisted.Splits.Should().HaveCount(2);
    }
}
