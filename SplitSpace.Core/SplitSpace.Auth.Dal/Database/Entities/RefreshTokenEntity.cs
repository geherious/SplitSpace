namespace SplitSpace.Auth.Dal.Database.Entities;

public record RefreshTokenEntity
{
    public required Guid Id { get; init; }
    
    public required Guid UserId { get; init; }
    
    public required string Token { get; init; }
    
    public required DateTimeOffset ExpiresAt { get; init; }
    
    public required DateTimeOffset? RevokedAt { get; init; }
    
    public required DateTimeOffset CreatedAt { get; init; }
}
