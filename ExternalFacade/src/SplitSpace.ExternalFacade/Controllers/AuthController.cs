using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitSpace.AuthService.Api.AuthService;
using SplitSpace.ExternalFacade.Dal.ClientFacades;
using SplitSpace.ExternalFacade.Logic.Models.Requests.Auth;
using SplitSpace.ExternalFacade.Logic.Models.Responses.Auth;

namespace SplitSpace.ExternalFacade.Controllers;

[ApiController]
[AllowAnonymous]
[Route("v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthServiceClientFacade _authServiceClientFacade;

    public AuthController(IAuthServiceClientFacade authServiceClientFacade)
    {
        _authServiceClientFacade = authServiceClientFacade;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterApiRequest apiRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new 
            { 
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        var grpcResponse = await _authServiceClientFacade.RegisterAsync(new RegisterRequest
        {
            Email = apiRequest.Email,
            Password = apiRequest.Password,
        });
        
        SetRefreshCookie(grpcResponse.RefreshToken);
        
        return Ok(new RegisterApiResponse
        {
            UserId = grpcResponse.UserId,
            AccessToken = grpcResponse.AccessToken
        });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginApiRequest apiRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new 
            { 
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        var grpcResponse = await _authServiceClientFacade.LoginAsync(new LoginRequest
        {
            Email = apiRequest.Email,
            Password = apiRequest.Password,
        });
        
        SetRefreshCookie(grpcResponse.RefreshToken);
        
        return Ok(new LoginApiResponse
        {
            UserId = grpcResponse.UserId,
            AccessToken = grpcResponse.AccessToken
        });
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new 
            { 
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }
        
        var refreshToken = GetRefreshToken();

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("Refresh token is empty");
        }

        var grpcResponse = await _authServiceClientFacade.RefreshTokenAsync(new RefreshTokenRequest
        {
            RefreshToken = refreshToken
        });
        
        SetRefreshCookie(grpcResponse.RefreshToken);
        
        return Ok(new RefreshApiResponse
        {
            AccessToken = grpcResponse.AccessToken
        });
    }

    private void SetRefreshCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,      // Prevents JavaScript access (security)
            Secure = true,        // Only send over HTTPS
            SameSite = SameSiteMode.Strict, // CSRF protection
            Expires = DateTimeOffset.UtcNow.AddDays(7), // 7 days expiry
            Path = "/",           // Available across entire site
        };
    
        Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
    }

    private string? GetRefreshToken()
    {
        Request.Cookies.TryGetValue("RefreshToken", out var refreshToken);
        return refreshToken;
    }
}