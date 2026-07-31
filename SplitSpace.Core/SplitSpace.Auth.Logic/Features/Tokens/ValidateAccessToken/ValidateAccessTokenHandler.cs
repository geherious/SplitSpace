using Mediator;
using SplitSpace.Auth.Domain.Services;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Logic.Features.Tokens.ValidateAccessToken;

public class ValidateAccessTokenHandler
    : ICommandHandler<ValidateAccessTokenCommand, Result<ValidateAccessTokenResultData>>
{
    private readonly IAccessTokenService _accessTokenService;

    public ValidateAccessTokenHandler(IAccessTokenService accessTokenService)
    {
        _accessTokenService = accessTokenService;
    }

    public async ValueTask<Result<ValidateAccessTokenResultData>> Handle(
        ValidateAccessTokenCommand command,
        CancellationToken cancellationToken)
    {
        var tokenValidationResult = _accessTokenService.Validate(command.AccessToken);

        if (tokenValidationResult.IsSuccess is false)
        {
            return Result<ValidateAccessTokenResultData>.Failure(
                new Error(ErrorType.Unauthenticated, "Invalid access token"));
        }
        
        return Result.Success(new ValidateAccessTokenResultData(tokenValidationResult.ResultValue.UserId));
    }
}
