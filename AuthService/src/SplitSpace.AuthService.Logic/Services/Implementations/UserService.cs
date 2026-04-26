using SplitSpace.AuthService.Common.Models;
using SplitSpace.AuthService.Dal.Repositories;
using SplitSpace.AuthService.Logic.Models.Commands;
using SplitSpace.AuthService.Logic.Models.Results;

namespace SplitSpace.AuthService.Logic.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserExistResultData>> Exist(UserExistCommand command)
    {
        var user = await _userRepository.FindByEmailAsync(command.Email);
        return Result<UserExistResultData>.Success(new UserExistResultData(user?.Id));
    }
}
