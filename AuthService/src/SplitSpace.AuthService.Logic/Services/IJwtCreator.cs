using SplitSpace.AuthService.Dal.Models.Entities;

namespace SplitSpace.AuthService.Logic.Services;

public interface IJwtCreator
{
    string CreateAccessToken(Guid userId);

    RefreshToken CreateRefreshToken(Guid userId);

    bool ValidateAccessToken(string token);

    bool ValidateRefreshToken(string token);
}
