using System.ComponentModel.DataAnnotations;

namespace SplitSpace.ExternalFacade.Logic.Models.Requests.Auth;

public class RegisterApiRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    public required string Password { get; set; }
}
