namespace SplitSpace.AuthService.Common.Models;

public record RefreshTokensResultData(
    string AccessToken,
    string RefreshToken
);
