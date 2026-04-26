namespace SplitSpace.AuthService.Common.Models.Commands;

public record RegisterCommand(
    string Email,
    string Password
);
