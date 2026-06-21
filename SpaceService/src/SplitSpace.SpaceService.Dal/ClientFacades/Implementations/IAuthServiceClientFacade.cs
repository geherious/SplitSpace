using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Dal.ClientFacades.Implementations;

public interface IAuthServiceClientFacade
{
    Task<UserId?> UserExistAsync(string email);
}
