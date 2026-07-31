using Mediator;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Logic.Features.Tokens.RefreshTokens;

public record RefreshTokenCommand(string RefreshToken) : ICommand<Result<RefreshTokenResultData>>;
