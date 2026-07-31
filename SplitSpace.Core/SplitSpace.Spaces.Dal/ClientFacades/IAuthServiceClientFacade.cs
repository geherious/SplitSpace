using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Dal.ClientFacades;

public interface IAuthServiceClientFacade
{
    Task<UserId?> UserExistAsync(string email);
}
