namespace SplitSpace.AuthService.Dal.Models.Entities;

public record RefreshToken
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required string Token { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
