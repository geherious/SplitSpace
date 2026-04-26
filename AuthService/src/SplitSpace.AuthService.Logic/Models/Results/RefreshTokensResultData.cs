namespace SplitSpace.AuthService.Logic.Models.Results;

public record RefreshTokensResultData(
    string AccessToken,
    string RefreshToken
);
