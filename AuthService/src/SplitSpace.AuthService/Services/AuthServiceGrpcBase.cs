using Grpc.Core;
using SplitSpace.AuthService.Api;
using SplitSpace.AuthService.Common.Models;
using SplitSpace.AuthService.Logic.Services;

namespace SplitSpace.AuthService.Services;

public class AuthServiceGrpcBase : Api.AuthService.AuthServiceBase
{
    private readonly IAuthService _authService;

    public AuthServiceGrpcBase(IAuthService authService)
    {
        _authService = authService;
    }

    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext  context)
    {
        var result = await _authService.Register(new RegisterCommand(request.Email, request.Password));
        
        if (!result.IsSuccess)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, result.Errors[0].Message));
        }

        return new RegisterResponse
        {
            UserId = result.Value.UserId.ToString(),
            AccessToken = result.Value.AccessToken,
            RefreshToken = result.Value.RefreshToken
        };
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext  context)
    {
        var result = await _authService.Login(new LoginCommand(request.Email, request.Password));
        
        if (!result.IsSuccess)
        {
            if (result.Errors[0].Type == ErrorType.Unauthenticated)
                throw new RpcException(
                    new Status(StatusCode.Unauthenticated, result.Errors[0].Message));
            
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, result.Errors[0].Message));
        }

        return new LoginResponse
        {
            UserId = result.Value.UserId.ToString(),
            AccessToken = result.Value.AccessToken,
            RefreshToken = result.Value.RefreshToken
        };
    }

    public override async Task<RefreshTokenResponse> RefreshTokens(RefreshTokenRequest request, ServerCallContext  context)
    {
        var result = await _authService.RefreshTokens(new RefreshTokensCommand(request.RefreshToken));
        
        if (!result.IsSuccess)
        {
            throw new RpcException(
                new Status(StatusCode.Unauthenticated, result.Errors[0].Message));
        }

        return new RefreshTokenResponse
        {
            AccessToken = result.Value.AccessToken,
            RefreshToken = result.Value.RefreshToken
        };
    }
}
