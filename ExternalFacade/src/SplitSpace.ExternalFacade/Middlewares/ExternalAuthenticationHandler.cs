using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SplitSpace.ExternalFacade.Logic.Models.Commands;
using SplitSpace.ExternalFacade.Logic.Services;

namespace SplitSpace.ExternalFacade.Middlewares;

public class ExternalAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ITokenService _tokenService;

    public ExternalAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ITokenService tokenService)
        : base(options, logger, encoder)
    {
        _tokenService = tokenService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authorization))
        {
            return AuthenticateResult.Fail("Missing authorization header");
        }

        var result = await _tokenService.AuthorizeUserAsync(
            new AuthorizeUserCommand(authorization));

        if (!result.IsSuccess)
        {
            return AuthenticateResult.Fail(result.Errors.First().Message);
        }
        
        Context.Items["UserId"] = result.Value.UserId;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, result.Value.UserId.ToString())
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
