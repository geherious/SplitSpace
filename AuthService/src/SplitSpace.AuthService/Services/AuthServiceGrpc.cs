using Grpc.Core;
using SplitSpace.AuthService.Api;
using SplitSpace.AuthService.Common.Models;
using SplitSpace.AuthService.Common.Models.Commands;
using SplitSpace.AuthService.Helpers;
using SplitSpace.AuthService.Logic.Services;

namespace SplitSpace.AuthService.Services;

public class AuthServiceGrpc : Api.AuthService.AuthServiceBase
{
    private readonly IAuthService _authService;

    public AuthServiceGrpc(IAuthService authService)
    {
        _authService = authService;
    }

    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext  context)
    {
        var result = await _authService.RegisterAsync(new RegisterCommand(request.Email, request.Password));
        
        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
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
        var result = await _authService.LoginAsync(new LoginCommand(request.Email, request.Password));
        
        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new LoginResponse
        {
            UserId = result.Value.UserId.ToString(),
            AccessToken = result.Value.AccessToken,
            RefreshToken = result.Value.RefreshToken
        };
    }

    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext  context)
    {
        var result = await _authService.RefreshTokenAsync(new RefreshTokensCommand(request.RefreshToken));
        
        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new RefreshTokenResponse
        {
            AccessToken = result.Value.AccessToken,
            RefreshToken = result.Value.RefreshToken
        };
    }

    public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
    {
        var result = await _authService.ValidateTokenAsync(new ValidateTokenCommand(request.AccessToken));
        
        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new ValidateTokenResponse
        {
            UserId = result.Value.UserId.ToString(),
        };
    }
}
