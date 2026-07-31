using System.Text;
using SplitSpace.SharedKernel.Domain.Exceptions;

namespace SplitSpace.Auth.Domain.Models.ValueObjects;

public readonly record struct RefreshTokenHash
{
    public string Value { get; }
    
    internal RefreshTokenHash(string value)
    {
        Value = value;
    }

    public static RefreshTokenHash FromRaw(string rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            throw new InvariantViolationException("Refresh token cannot be empty");

        return new RefreshTokenHash(Sha256(rawToken));
    }
    
    public static (RefreshTokenHash refreshTokenHash, string rawToken) Generate()
    {
        var rawToken = Guid.NewGuid().ToString("N");
        var hash = Sha256(rawToken);
        
        var refreshTokenHash = new RefreshTokenHash(hash);
        return (refreshTokenHash, rawToken);
    }
    
    private static string Sha256(string input)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}
