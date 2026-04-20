namespace SplitSpace.AuthService.Common.Models;

public record RegisterResultData(
    Guid UserId,
    string AccessToken,
    string RefreshToken
);
