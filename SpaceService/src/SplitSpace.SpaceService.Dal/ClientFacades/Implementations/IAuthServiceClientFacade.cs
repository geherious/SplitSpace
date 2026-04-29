namespace SplitSpace.SpaceService.Dal.ClientFacades.Implementations;

public interface IAuthServiceClientFacade
{
    Task<Guid?> UserExistAsync(string email);
}
