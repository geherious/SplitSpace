using System.ComponentModel.DataAnnotations;

namespace SplitSpace.ExternalFacade.Logic.Models.Requests.Finance;

public class GetPersonalAccountsApiRequest
{
    [Required]
    public required Guid UserId { get; set; }
}
