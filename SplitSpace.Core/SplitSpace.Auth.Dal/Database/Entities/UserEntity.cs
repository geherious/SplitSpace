namespace SplitSpace.Auth.Dal.Database.Entities;

public record UserEntity
{
    public required Guid Id { get; init; }
    
    public required string Email { get; init; }
    
    public required string PasswordHash { get; init; }
    
    public required DateTimeOffset CreatedAt { get; init; }
    
    public required DateTimeOffset? LastLogin { get; init; }
}
