using SplitSpace.FinanceService.Common.Enums;
using SplitSpace.FinanceService.Common.Models;
using SplitSpace.FinanceService.Dal.Models.Entities;
using SplitSpace.FinanceService.Dal.Repositories;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Models.Results;

namespace SplitSpace.FinanceService.Logic.Services.Implementations;

public class DebtService : IDebtService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDebtRepository _debtRepository;
    private readonly ISpaceMembershipRepository _spaceMembershipRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ISettlementRepository _settlementRepository;

    public DebtService(IUnitOfWork unitOfWork,
        IDebtRepository debtRepository,
        ISpaceMembershipRepository spaceMembershipRepository,
        IAccountRepository accountRepository,
        ISettlementRepository settlementRepository)
    {
        _unitOfWork = unitOfWork;
        _debtRepository = debtRepository;
        _spaceMembershipRepository = spaceMembershipRepository;
        _accountRepository = accountRepository;
        _settlementRepository = settlementRepository;
    }

    public async Task<Result<GetDebtsResultData>> GetDebtsAsync(GetDebtsCommand command)
    {
        var membership = await _spaceMembershipRepository.GetAsync(command.SpaceId, command.UserId);

        if (membership is null)
        {
            return Result<GetDebtsResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Space not found"
            });
        }

        var debts = await _debtRepository.GetBatchAsync(command.SpaceId, command.UserId);
        
        return Result.Success(new GetDebtsResultData(debts));
    }

    public async Task<Result> SettleDebt(SettleDebtCommand command)
    {
        if (command.Amount <= 0)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.Validation,
                Message = "Settlement amount must be greater than zero"
            });
        }
        
        var userMembership = await _spaceMembershipRepository.GetAsync(command.SpaceId, command.UserId);

        if (userMembership is null)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Space not found"
            });
        }
        
        var userToMembership = await _spaceMembershipRepository.GetAsync(command.SpaceId, command.ToUserId);

        if (userToMembership is null)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Destination user not found"
            });
        }
        
        var account = await _accountRepository.GetAsync(command.AccountId);
        
        if (account is null ||
            (account.OwnerType == AccountOwnerType.Personal && account.OwnerId != command.UserId) ||
            (account.OwnerType == AccountOwnerType.Space && account.OwnerId != command.SpaceId))
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Account not found"
            });
        }

        if (account.OwnerType == AccountOwnerType.Space)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.FailedPrecondition,
                Message = "Cannot pay split from space account"
            });
        }

        var settlement = new Settlement
        {
            Id = Guid.CreateVersion7(),
            FromUserId = command.UserId,
            FromAccountId = command.AccountId,
            ToUserId = command.ToUserId,
            Amount = command.Amount,
            CreatedAt = DateTimeOffset.UtcNow
        };
        
        var debtFrom = command.UserId;
        var debtTo = command.ToUserId;
        var debtAmount = -command.Amount;

        if (debtFrom > debtTo)
        {
            (debtFrom, debtTo) = (debtTo, debtFrom);
            debtAmount *= -1;
        }

        var debt = new Debt
        {
            Id = Guid.CreateVersion7(),
            SpaceId = command.SpaceId,
            FromUserId = debtFrom,
            ToUserId = debtTo,
            Amount = debtAmount
        };

        await _settlementRepository.AddAsync(settlement);
        await _debtRepository.AddOrUpdateAsync([debt]);
        await _accountRepository.UpdateAmountAsync(command.AccountId, -command.Amount);
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }
}