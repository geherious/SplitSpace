using System.ComponentModel.DataAnnotations;

namespace SplitSpace.ExternalFacade.Logic.Models.Requests.Finance;

public class GetCategoryReportApiRequest
{
    [Required]
    public required DateOnly From { get; set; }
    [Required]
    public required DateOnly To { get; set; }
    [Required]
    public required string Timezone { get; set; }
}
