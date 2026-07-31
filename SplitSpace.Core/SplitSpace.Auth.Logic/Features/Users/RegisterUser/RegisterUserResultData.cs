using SplitSpace.Auth.Domain.Models.Ids;

namespace SplitSpace.Auth.Logic.Features.Users.RegisterUser;

public record RegisterUserResultData(
    UserId UserId,
    string AccessToken,
    string RefreshToken);
    