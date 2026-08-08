using SplitSpace.Finances.Domain.Models.Events;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Domain.Models;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.SettlementAggregate;

public sealed record Settlement : AggregateRoot<SettlementId>
{
    public override SettlementId Id { get; protected set; }

    public UserId FromUserId { get; private set; }

    public BalanceId FromBalanceId { get; private set; }

    public UserId ToUserId { get; private set; }

    public Money Amount { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private Settlement(
        SettlementId id,
        UserId fromUserId,
        BalanceId fromBalanceId,
        UserId toUserId,
        Money amount,
        DateTimeOffset createdAt)
    {
        Id = id;
        FromUserId = fromUserId;
        FromBalanceId = fromBalanceId;
        ToUserId = toUserId;
        Amount = amount;
        CreatedAt = createdAt;
    }

    private Settlement() { }

    public static Result<Settlement> Create(
        UserId fromUserId,
        BalanceId fromBalanceId,
        UserId toUserId,
        Money amount,
        DateTimeOffset createdAt)
    {
        if (fromUserId == toUserId)
        {
            return Result<Settlement>.Failure(new Error(ErrorType.Validation, "Cannot settle a debt with yourself"));
        }

        if (amount.Amount <= 0)
        {
            return Result<Settlement>.Failure(new Error(ErrorType.Validation, "Settlement amount must be greater than zero"));
        }

        var settlement = new Settlement(
            SettlementId.New(),
            fromUserId,
            fromBalanceId,
            toUserId,
            amount,
            createdAt);

        settlement.AddDomainEvent(new SettlementCreatedEvent(settlement));

        return Result.Success(settlement);
    }

    public static Settlement Rehydrate(
        SettlementId id,
        UserId fromUserId,
        BalanceId fromBalanceId,
        UserId toUserId,
        Money amount,
        DateTimeOffset createdAt)
    {
        return new Settlement
        {
            Id = id,
            FromUserId = fromUserId,
            FromBalanceId = fromBalanceId,
            ToUserId = toUserId,
            Amount = amount,
            CreatedAt = createdAt
        };
    }
}
