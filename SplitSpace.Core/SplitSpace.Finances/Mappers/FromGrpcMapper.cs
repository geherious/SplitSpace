using Grpc.Core;
using SplitSpace.Finances.Api.FinanceService;
using SplitSpace.Finances.Domain.Models.Aggregates.ExpenseAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.Finances.Helpers;

namespace SplitSpace.Finances.Mappers;

public static class FromGrpcMapper
{
    public static ExpenseSplitMethod ToCommand(this AddExpenseRequest.Types.Split split)
    {
        return split.TypeCase switch
        {
            AddExpenseRequest.Types.Split.TypeOneofCase.None =>
                new ExpenseSplitMethod.None(),
            AddExpenseRequest.Types.Split.TypeOneofCase.EqualSplit =>
                new ExpenseSplitMethod.EqualSplit
                {
                    Participants = split.EqualSplit.UserIds
                        .Select(userId => new ExpenseSplitMethod.EqualSplit.SplitParticipant(
                            new UserId(userId.ToGuidOrThrow(nameof(AddExpenseRequest.Types.SplitParticipant.UserId)))))
                        .ToArray()
                },
            AddExpenseRequest.Types.Split.TypeOneofCase.ExactSplit =>
                new ExpenseSplitMethod.ExactAmount
                {
                    Participants = split.ExactSplit.Participants
                        .Select(participant => new ExpenseSplitMethod.ExactAmount.SplitParticipant(
                            new UserId(participant.UserId.ToGuidOrThrow(nameof(AddExpenseRequest.Types.SplitParticipant.UserId))),
                            new Money(participant.Amount.ToDecimalOrThrow(nameof(AddExpenseRequest.Types.SplitParticipant.Amount)))))
                        .ToArray()
                },
            AddExpenseRequest.Types.Split.TypeOneofCase.PercentageSplit =>
                new ExpenseSplitMethod.Percentages
                {
                    Participants = split.PercentageSplit.Participants
                        .Select(participant => new ExpenseSplitMethod.Percentages.SplitParticipant(
                            new UserId(participant.UserId.ToGuidOrThrow(nameof(AddExpenseRequest.Types.SplitParticipant.UserId))),
                            Convert.ToDecimal(participant.Percent)))
                        .ToArray()
                },
            _ => throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid split type"))
        };
    }
}
