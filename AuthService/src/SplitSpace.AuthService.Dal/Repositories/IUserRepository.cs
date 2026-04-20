using SplitSpace.AuthService.Dal.Models.Entities;

namespace SplitSpace.AuthService.Dal.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> FindByEmailAsync(string email);
    Task CreateUserAsync(User user);
    Task UpdateLastLoginAsync(Guid userId);
}
