using SplitSpace.AuthService.Api.AuthService;

namespace SplitSpace.ExternalFacade.Dal.ClientFacades.Implementations;

public class AuthServiceClientFacade : IAuthServiceClientFacade
{
    private readonly AuthService.Api.AuthService.AuthService.AuthServiceClient _authService;

    public AuthServiceClientFacade(AuthService.Api.AuthService.AuthService.AuthServiceClient authService)
    {
        _authService = authService;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        
        return response;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        
        return response;
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request);
        
        return response;
    }

    public async Task<ValidateTokenResponse> ValidateTokenAsync(ValidateTokenRequest request)
    {
        var response = await _authService.ValidateTokenAsync(request);
        
        return response;
    }
}
