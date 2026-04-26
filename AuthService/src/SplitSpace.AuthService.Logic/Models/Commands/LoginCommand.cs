namespace SplitSpace.AuthService.Logic.Models.Commands;

public record LoginCommand(
    string Email,
    string Password
);
