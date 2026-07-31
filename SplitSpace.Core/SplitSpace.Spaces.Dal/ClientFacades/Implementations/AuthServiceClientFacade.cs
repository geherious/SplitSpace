using Mediator;
using SplitSpace.Auth.Logic.Features.Users.UserExist;
using SplitSpace.Spaces.Domain.Models.Ids;

namespace SplitSpace.Spaces.Dal.ClientFacades.Implementations;

public class AuthServiceClientFacade : IAuthServiceClientFacade
{
    private readonly IMediator _mediator;

    public AuthServiceClientFacade(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<UserId?> UserExistAsync(string email)
    {
        var result = await _mediator.Send(new UserExistCommand(email));

        if (result.IsSuccess is false || result.ResultValue?.UserId is not { } userId)
        {
            return null;
        }

        return new UserId(userId.Value);
    }
}
