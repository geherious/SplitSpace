using Mediator;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Logic.Features.Users.UserExist;

public record UserExistCommand(string Email) : ICommand<Result<UserExistResultData>>;
