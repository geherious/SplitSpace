using SplitSpace.AuthService.Common;
using SplitSpace.AuthService.Common.Models;
using SplitSpace.AuthService.Logic.Models.Commands;
using SplitSpace.AuthService.Logic.Models.Results;

namespace SplitSpace.AuthService.Logic.Services;

public interface IAuthService
{
    Task<Result<RegisterResultData>> RegisterAsync(RegisterCommand command);
    Task<Result<LoginResultData>> LoginAsync(LoginCommand command);
    Task<Result<RefreshTokensResultData>> RefreshTokenAsync(RefreshTokensCommand command);
    Task<Result<ValidateTokenResultData>> ValidateTokenAsync(ValidateTokenCommand command);
}
