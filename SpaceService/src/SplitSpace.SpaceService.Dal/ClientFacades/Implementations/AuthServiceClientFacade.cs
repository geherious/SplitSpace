using SplitSpace.AuthService.Api.UserService;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Dal.ClientFacades.Implementations;

public class AuthServiceClientFacade : IAuthServiceClientFacade
{
    private readonly UserService.UserServiceClient _userClient;

    public AuthServiceClientFacade(UserService.UserServiceClient userClient)
    {
        _userClient = userClient;
    }

    public async Task<UserId?> UserExistAsync(string email)
    {
        var result = await _userClient.ExistAsync(new ExistRequest{ Email = email });

        if (result.UserId == null)
        {
            return null;
        }

        if (Guid.TryParse(result.UserId, out var userId) is false)
        {
            throw new ArgumentException($"Invalid user id: {result.UserId}");
        }
        
        return new UserId(userId);
    }
}
