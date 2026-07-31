using SplitSpace.Auth.Domain.Models.Ids;
using SplitSpace.Auth.Domain.Models.ValueObjects;

namespace SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;

public interface IUserDomainRepository
{
    Task<User?> GetAsync(UserId userId, CancellationToken cancellationToken);

    Task<User?> GetAsync(Email email, CancellationToken cancellationToken);
    
    Task SaveAsync(User user, CancellationToken cancellationToken);
}
