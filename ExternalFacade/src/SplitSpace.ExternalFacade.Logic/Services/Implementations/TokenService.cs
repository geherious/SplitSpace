using SplitSpace.AuthService.Api.AuthService;
using SplitSpace.ExternalFacade.Common.Models;
using SplitSpace.ExternalFacade.Dal.ClientFacades;
using SplitSpace.ExternalFacade.Logic.Models.Commands;
using SplitSpace.ExternalFacade.Logic.Models.Results;

namespace SplitSpace.ExternalFacade.Logic.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly IAuthServiceClientFacade _authServiceClientFacade;

    public TokenService(IAuthServiceClientFacade authServiceClientFacade)
    {
        _authServiceClientFacade = authServiceClientFacade;
    }

    public async Task<Result<AuthorizeUserResultData>> AuthorizeUserAsync(
        AuthorizeUserCommand command)
    {
        if (string.IsNullOrEmpty(command.AccessTokenHeader))
        {
            return Result<AuthorizeUserResultData>.Failure(new Error
            {
                Type = ErrorType.Unauthenticated,
                Message = "Authorization header is missing"
            });
        }
        
        const string bearerPrefix = "Bearer ";
        if (command.AccessTokenHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase) is false)
        {
            return Result<AuthorizeUserResultData>.Failure(new Error
            {
                Type = ErrorType.Unauthenticated,
                Message = "Invalid authorization header"
            });
        }
        
        var token = command.AccessTokenHeader[bearerPrefix.Length..].Trim();

        var authResponse = await _authServiceClientFacade.ValidateTokenAsync(new ValidateTokenRequest
        {
            AccessToken = token
        });
        
        var userId = Guid.Parse(authResponse.UserId);

        return Result.Success(new AuthorizeUserResultData(userId));
    }
}
