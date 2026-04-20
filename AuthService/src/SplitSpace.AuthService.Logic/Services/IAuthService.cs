using SplitSpace.AuthService.Common;
using SplitSpace.AuthService.Common.Models;

namespace SplitSpace.AuthService.Logic.Services;

public interface IAuthService
{
    Task<Result<RegisterResultData>> Register(RegisterCommand command);
    Task<Result<LoginResultData>> Login(LoginCommand command);
    Task<Result<RefreshTokensResultData>> RefreshTokens(RefreshTokensCommand command);
}
