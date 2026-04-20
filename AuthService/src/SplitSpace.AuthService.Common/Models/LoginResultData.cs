namespace SplitSpace.AuthService.Common.Models;

public record LoginResultData(
    Guid UserId,
    string AccessToken,
    string RefreshToken
);
