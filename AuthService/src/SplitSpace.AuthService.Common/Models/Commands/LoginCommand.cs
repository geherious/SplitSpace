namespace SplitSpace.AuthService.Common.Models.Commands;

public record LoginCommand(
    string Email,
    string Password
);
