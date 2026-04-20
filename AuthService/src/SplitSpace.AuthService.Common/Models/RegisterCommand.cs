namespace SplitSpace.AuthService.Common.Models;

public record RegisterCommand(
    string Email,
    string Password
);
