using SplitSpace.Finances.Domain.Models.Events;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Domain.Models;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;

public sealed record Expense : AggregateRoot<ExpenseId>
{
    public override ExpenseId Id { get; protected set; }
    
    public SpaceId SpaceId { get; private set; }
    
    public UserId CreatedBy { get; private set; }
    
    public CategoryId CategoryId { get; private set; }
    
    public BalanceId BalanceId { get; private set; }
    
    public Money Amount { get; private set; }
    
    public string Description { get; private set; } = string.Empty;
    
    public DateTimeOffset CreatedAt { get; private set; }
    
    public IReadOnlyList<ExpenseSplit> ExpenseSplits { get; private set; } = Array.Empty<ExpenseSplit>();

    private Expense(
        ExpenseId id,
        SpaceId spaceId,
        UserId createdBy,
        CategoryId categoryId,
        BalanceId balanceId,
        Money amount,
        string description,
        DateTimeOffset createdAt,
        IReadOnlyList<ExpenseSplit> expenseSplits)
    {
        Id = id;
        SpaceId = spaceId;
        CreatedBy = createdBy;
        CategoryId = categoryId;
        BalanceId = balanceId;
        Amount = amount;
        Description = description;
        CreatedAt = createdAt;
        ExpenseSplits = expenseSplits;
    }

    public static Result<Expense> Create(
        SpaceId spaceId,
        UserId createdBy,
        CategoryId categoryId,
        BalanceId balanceId,
        Money amount,
        string description,
        DateTimeOffset currentTime,
        IExpenseSplitPolicy splitPolicy)
    {
        var expenseId = ExpenseId.New();
        var createdSplits = splitPolicy.CalculateSplits(expenseId, spaceId, amount);
        if (createdSplits.IsSuccess is false)
        {
            return Result<Expense>.Failure(createdSplits.Errors);
        }

        var expense = new Expense(
            expenseId,
            spaceId,
            createdBy,
            categoryId,
            balanceId,
            amount,
            description,
            currentTime,
            createdSplits.ResultValue);

        expense.AddDomainEvent(new ExpenseCreatedEvent(expense));

        return Result.Success(expense);
    }
}
