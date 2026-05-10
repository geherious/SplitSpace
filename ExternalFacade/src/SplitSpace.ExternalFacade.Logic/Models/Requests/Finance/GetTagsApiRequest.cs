using System.ComponentModel.DataAnnotations;

namespace SplitSpace.ExternalFacade.Logic.Models.Requests.Finance;

public class GetTagsApiRequest
{
    [Required]
    public required Guid SpaceId { get; set; }
}
