using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;
using SplitSpace.Auth.Domain.Services;
using SplitSpace.Auth.Logic.Constants;
using SplitSpace.Auth.Logic.Options;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Logic.Services;

public class AccessTokenService : IAccessTokenService
{
    private readonly IOptionsMonitor<JwtOptions> _options;
    private readonly ILogger<AccessTokenService> _logger;

    public AccessTokenService(
        IOptionsMonitor<JwtOptions> options,
        ILogger<AccessTokenService> logger)
    {
        _options = options;
        _logger = logger;
    }

    public string Generate(User user)
    {
        var jwtOptions = _options.CurrentValue;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(RegisteredJwtClaims.Sub, user.Id.Value.ToString()),
            new(RegisteredJwtClaims.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Result<AccessTokenValidationResultData> Validate(string token)
    {
        var jwtOptions = _options.CurrentValue;
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var key = Encoding.UTF8.GetBytes(jwtOptions.Secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == RegisteredJwtClaims.Sub);
            if (userIdClaim is null ||
                string.IsNullOrEmpty(userIdClaim.Value) ||
                Guid.TryParse(userIdClaim.Value, out var userId) is false)
            {
                return Result<AccessTokenValidationResultData>.Failure(
                    new Error(ErrorType.Unauthenticated, "Invalid access token"));
            }

            return Result<AccessTokenValidationResultData>.Success(
                new AccessTokenValidationResultData(userId));
        }
        catch (Exception ex)
        {
            _logger.LogError("failed to validate access token: {Message}", ex.Message);
            return Result<AccessTokenValidationResultData>.Failure(
                new Error(ErrorType.Unauthenticated, "Invalid access token"));
        }
    }
}
