using SplitSpace.FinanceService.Common.Enums;
using SplitSpace.FinanceService.Common.Models;
using SplitSpace.FinanceService.Dal.Models.Entities;
using SplitSpace.FinanceService.Dal.Repositories;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Models.Results;
using SplitSpace.SpaceService.Dal.Repositories;

namespace SplitSpace.FinanceService.Logic.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountRepository _accountRepository;
    private readonly ISpaceMembershipRepository _spaceMembershipRepository;

    public AccountService(IAccountRepository accountRepository,
        ISpaceMembershipRepository spaceMembershipRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _spaceMembershipRepository = spaceMembershipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddAccountResultData>> AddAccountAsync(AddAccountCommand command)
    {
        if (command.AccountOwnerType == AccountOwnerType.Space)
        {
            var membership = await _spaceMembershipRepository.GetAsync(command.OwnerId, command.UserId);

            if (membership is null)
            {
                return Result<AddAccountResultData>.Failure(new Error
                {
                    Type = ErrorType.NotFound,
                    Message = "Space not found"
                });
            }
        }

        if (command.AccountOwnerType == AccountOwnerType.Personal &&
            command.OwnerId != command.UserId)
        {
            return Result<AddAccountResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Owner id should equal the user id"
            });
        }

        var account = new Account
        {
            Id = Guid.CreateVersion7(),
            Name = command.Name,
            Balance = command.Balance,
            OwnerType = command.AccountOwnerType,
            OwnerId = command.OwnerId
        };

        await _accountRepository.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();

        return Result<AddAccountResultData>.Success(new AddAccountResultData(account.Id));
    }

    public async Task<Result<GetSpaceAccountsResultData>> GetSpaceAccountsAsync(GetSpaceAccountsCommand command)
    {
        var membership = await _spaceMembershipRepository.GetAsync(command.SpaceId, command.UserId);

        if (membership is null)
        {
            return Result<GetSpaceAccountsResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Space not found"
            });
        }

        var spaceAccounts = await _accountRepository.GetSpaceAccountBatchAsync(command.SpaceId);
        var mappedAccounts = spaceAccounts
            .Select(a => new GetSpaceAccountsResultData.Account
            {
                Id = a.Id,
                Name = a.Name,
                Balance = a.Balance,
                SpaceId = a.OwnerId
            })
            .ToArray();

        return Result.Success(new GetSpaceAccountsResultData(mappedAccounts));
    }

    public async Task<Result<GetPersonalAccountsResultData>> GetPersonalAccountsAsync(GetPersonalAccountsCommand command)
    {
        var spaceAccounts = await _accountRepository.GetUserAccountBatchAsync(command.UserId);
        var mappedAccounts = spaceAccounts
            .Select(a => new GetPersonalAccountsResultData.Account
            {
                Id = a.Id,
                Name = a.Name,
                Balance = a.Balance,
                UserId = a.OwnerId
            })
            .ToArray();

        return Result.Success(new GetPersonalAccountsResultData(mappedAccounts));
    }
}
