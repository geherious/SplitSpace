using Mediator;
using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;
using SplitSpace.Auth.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Logic.Features.Users.UserExist;

public class UserExistHandler : ICommandHandler<UserExistCommand, Result<UserExistResultData>>
{
    private readonly IUserDomainRepository _userDomainRepository;

    public UserExistHandler(IUserDomainRepository userDomainRepository)
    {
        _userDomainRepository = userDomainRepository;
    }

    public async ValueTask<Result<UserExistResultData>> Handle(
        UserExistCommand command,
        CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsSuccess is false)
        {
            return Result<UserExistResultData>.Failure(emailResult.Errors);
        }

        var user = await _userDomainRepository.GetAsync(emailResult.ResultValue, cancellationToken);

        return Result<UserExistResultData>.Success(new UserExistResultData(user?.Id));
    }
}
