namespace SplitSpace.AuthService.Logic.Services.Implementations;

public class PasswordHasher
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
