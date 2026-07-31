using Mediator;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Logic.Features.Tokens.ValidateAccessToken;

public record ValidateAccessTokenCommand(string AccessToken) : ICommand<Result<ValidateAccessTokenResultData>>;
