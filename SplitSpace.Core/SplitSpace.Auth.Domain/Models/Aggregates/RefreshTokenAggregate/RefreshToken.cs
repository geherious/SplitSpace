using SplitSpace.Auth.Domain.Models.Events;
using SplitSpace.Auth.Domain.Models.Ids;
using SplitSpace.Auth.Domain.Models.ValueObjects;
using SplitSpace.SharedKernel.Domain.Models;

namespace SplitSpace.Auth.Domain.Models.Aggregates.RefreshTokenAggregate;

public sealed record RefreshToken : AggregateRoot<RefreshTokenId>
{
    public override RefreshTokenId Id { get; protected set; }
    
    public UserId UserId { get; private set; }
    
    public RefreshTokenHash Token { get; private set; }
    
    public DateTimeOffset ExpiresAt { get; private set; }
    
    public DateTimeOffset? RevokedAt { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }

    private RefreshToken(
        RefreshTokenId id,
        UserId userId,
        RefreshTokenHash token,
        DateTimeOffset expiresAt,
        DateTimeOffset? revokedAt,
        DateTimeOffset createdAt)
    {
        Id = id;
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        RevokedAt = revokedAt;
        CreatedAt = createdAt;
    }
    
    private RefreshToken() {}

    public static RefreshToken Rehydrate(
        RefreshTokenId id,
        UserId userId,
        RefreshTokenHash token,
        DateTimeOffset expiresAt,
        DateTimeOffset? revokedAt,
        DateTimeOffset createdAt)
    {
        return new RefreshToken
        {
            Id = id,
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            RevokedAt = revokedAt,
            CreatedAt = createdAt,
        };
    }

    public static (RefreshToken refreshToken, string rawToken) Create(
        UserId userId,
        DateTimeOffset createdAt)
    {
        var expiresAt = createdAt.Add(TimeSpan.FromDays(15));
        
        var (refreshTokenHash, rawToken) = RefreshTokenHash.Generate();
        
        var refreshToken = new RefreshToken(
            RefreshTokenId.New(),
            userId,
            refreshTokenHash,
            expiresAt,
            null,
            createdAt);

        refreshToken.AddDomainEvent(new RefreshTokenCreatedEvent(refreshToken));

        return (refreshToken, rawToken);
    }

    public bool IsValid(DateTimeOffset currentTime)
    {
        if (this.ExpiresAt < currentTime)
        {
            return false;
        }
        
        return true;
    }

    public void Revoke(DateTimeOffset revokedAt)
    {
        RevokedAt = revokedAt;
        AddDomainEvent(new RefreshTokenRevokedEvent(this));
    }
}
