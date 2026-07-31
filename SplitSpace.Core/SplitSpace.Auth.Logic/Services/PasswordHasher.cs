using SplitSpace.Auth.Domain.Services;

namespace SplitSpace.Auth.Logic.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltRounds = 12;

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, SaltRounds);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
