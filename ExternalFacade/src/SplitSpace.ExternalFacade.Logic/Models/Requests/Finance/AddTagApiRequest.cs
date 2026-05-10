using System.ComponentModel.DataAnnotations;

namespace SplitSpace.ExternalFacade.Logic.Models.Requests.Finance;

public class AddTagApiRequest
{
    [Required]
    public required Guid SpaceId { get; set; }

    [Required]
    public required string Name { get; set; }
}
