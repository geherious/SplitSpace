using SplitSpace.AuthService.Common.Models;
using SplitSpace.AuthService.Logic.Models.Commands;
using SplitSpace.AuthService.Logic.Models.Results;

namespace SplitSpace.AuthService.Logic.Services;

public interface IUserService
{
    Task<Result<UserExistResultData>> Exist(UserExistCommand command);
}