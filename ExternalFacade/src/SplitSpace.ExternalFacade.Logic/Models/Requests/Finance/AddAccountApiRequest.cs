using System.ComponentModel.DataAnnotations;
using SplitSpace.ExternalFacade.Logic.Models.Enums;

namespace SplitSpace.ExternalFacade.Logic.Models.Requests.Finance;

public class AddAccountApiRequest
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public decimal Balance { get; set; }
}
