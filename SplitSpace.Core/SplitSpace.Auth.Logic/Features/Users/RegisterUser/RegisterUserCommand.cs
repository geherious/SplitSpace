using Mediator;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Logic.Features.Users.RegisterUser;

public record RegisterUserCommand(string Email, string Password) : ICommand<Result<RegisterUserResultData>>;
