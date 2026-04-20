namespace SplitSpace.AuthService.Common.Models;

public record LoginCommand(
    string Email,
    string Password
);
