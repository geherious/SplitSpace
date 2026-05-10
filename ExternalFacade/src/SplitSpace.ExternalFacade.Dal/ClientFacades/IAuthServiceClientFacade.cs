using SplitSpace.AuthService.Api.AuthService;

namespace SplitSpace.ExternalFacade.Dal.ClientFacades;

public interface IAuthServiceClientFacade
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ValidateTokenResponse> ValidateTokenAsync(ValidateTokenRequest request);
}
