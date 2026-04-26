using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SplitSpace.AuthService.Dal.Models.Entities;
using SplitSpace.AuthService.Logic.Options;

namespace SplitSpace.AuthService.Logic.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly IOptionsMonitor<JwtOptions> _config;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IOptionsMonitor<JwtOptions> config,
        ILogger<TokenService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public string CreateAccessToken(IReadOnlyCollection<Claim> claims)
    {
        var jwtOptions = _config.CurrentValue;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken CreateRefreshToken(Guid userId)
    {
        var jwtOptions = _config.CurrentValue;

        var expiresAt = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenExpiryDays);
        
        var randomBytes = new byte[64]; // 512 bits
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var token = Convert.ToBase64String(randomBytes);

        return new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        var jwtOptions = _config.CurrentValue;
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler
            {
                MapInboundClaims = false
            };
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
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError("failed to validate token: exception: {Message}", ex.Message);
            return null;
        }
    }
}
