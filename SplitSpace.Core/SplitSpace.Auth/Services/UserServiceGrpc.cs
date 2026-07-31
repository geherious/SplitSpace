using Grpc.Core;
using Mediator;
using SplitSpace.Auth.Api.UserService;
using SplitSpace.Auth.Helpers;
using SplitSpace.Auth.Logic.Features.Users.UserExist;

namespace SplitSpace.Auth.Services;

public class UserServiceGrpc : UserService.UserServiceBase
{
    private readonly IMediator _mediator;

    public UserServiceGrpc(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<ExistResponse> Exist(ExistRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(
            new UserExistCommand(request.Email),
            context.CancellationToken);

        if (result.IsSuccess is false)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new ExistResponse
        {
            UserId = result.ResultValue.UserId?.Value.ToString()
        };
    }
}
