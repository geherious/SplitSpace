using Microsoft.EntityFrameworkCore;
using SplitSpace.AuthService.Dal.Models.Entities;
using SplitSpace.AuthService.Dal.Repositories;

namespace SplitSpace.AuthService.Dal.repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly AuthServiceDbContext _context;

    public UserRepository(AuthServiceDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Equals(email));
    }

    public async Task CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLastLoginAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
