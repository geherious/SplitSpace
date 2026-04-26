using Grpc.Core;
using SplitSpace.AuthService.Api.UserService;
using SplitSpace.AuthService.Helpers;
using SplitSpace.AuthService.Logic.Models.Commands;
using SplitSpace.AuthService.Logic.Services;

namespace SplitSpace.AuthService.Services;

public class UserServiceGrpc : UserService.UserServiceBase
{
    private readonly IUserService _userService;

    public UserServiceGrpc(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<ExistResponse> Exist(ExistRequest request, ServerCallContext context)
    {
        var result = await _userService.Exist(new UserExistCommand(request.Email));
        
        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new ExistResponse
        {
            UserId = result.Value.UserId?.ToString()
        };
    }
}
