using System.Security.Claims;
using SplitSpace.AuthService.Dal.Models.Entities;

namespace SplitSpace.AuthService.Logic.Services;

public interface ITokenService
{
    string CreateAccessToken(IReadOnlyCollection<Claim> claims);

    RefreshToken CreateRefreshToken(Guid userId);

    ClaimsPrincipal? ValidateAccessToken(string token);
}
