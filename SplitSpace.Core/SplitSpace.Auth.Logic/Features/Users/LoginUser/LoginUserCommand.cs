using Mediator;
using SplitSpace.Auth.Logic.Features.Users.RegisterUser;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Logic.Features.Users.LoginUser;

public record LoginUserCommand(string Email, string Password) : ICommand<Result<LoginUserResultData>>;
