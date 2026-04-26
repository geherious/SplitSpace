namespace SplitSpace.AuthService.Logic.Models.Commands;

public record RegisterCommand(
    string Email,
    string Password
);
