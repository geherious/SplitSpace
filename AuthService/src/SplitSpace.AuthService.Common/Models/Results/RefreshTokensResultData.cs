namespace SplitSpace.AuthService.Common.Models.Results;

public record RefreshTokensResultData(
    string AccessToken,
    string RefreshToken
);
