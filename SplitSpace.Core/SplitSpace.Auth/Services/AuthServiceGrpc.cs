using Grpc.Core;
using Mediator;
using SplitSpace.Auth.Api.AuthService;
using SplitSpace.Auth.Helpers;
using SplitSpace.Auth.Logic.Features.Tokens.RefreshTokens;
using SplitSpace.Auth.Logic.Features.Tokens.ValidateAccessToken;
using SplitSpace.Auth.Logic.Features.Users.LoginUser;
using SplitSpace.Auth.Logic.Features.Users.RegisterUser;

namespace SplitSpace.Auth.Services;

public class AuthServiceGrpc : AuthService.AuthServiceBase
{
    private readonly IMediator _mediator;

    public AuthServiceGrpc(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(
            new RegisterUserCommand(request.Email, request.Password),
            context.CancellationToken);

        if (result.IsSuccess is false)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new RegisterResponse
        {
            UserId = result.ResultValue.UserId.Value.ToString(),
            AccessToken = result.ResultValue.AccessToken,
            RefreshToken = result.ResultValue.RefreshToken
        };
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(
            new LoginUserCommand(request.Email, request.Password),
            context.CancellationToken);

        if (result.IsSuccess is false)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new LoginResponse
        {
            UserId = result.ResultValue.UserId.Value.ToString(),
            AccessToken = result.ResultValue.AccessToken,
            RefreshToken = result.ResultValue.RefreshToken
        };
    }

    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(
            new RefreshTokenCommand(request.RefreshToken),
            context.CancellationToken);

        if (result.IsSuccess is false)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new RefreshTokenResponse
        {
            AccessToken = result.ResultValue.AccessToken,
            RefreshToken = result.ResultValue.RefreshToken
        };
    }

    public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(
            new ValidateAccessTokenCommand(request.AccessToken),
            context.CancellationToken);

        if (result.IsSuccess is false)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new ValidateTokenResponse
        {
            UserId = result.ResultValue.UserId.ToString()
        };
    }
}
