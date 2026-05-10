using System.ComponentModel.DataAnnotations;
using SplitSpace.FinanceService.Api.Common;
using Google.Protobuf.WellKnownTypes;

namespace SplitSpace.ExternalFacade.Logic.Models.Requests.Finance;

public class AddCategoryApiRequest
{
    [Required]
    public required Guid SpaceId { get; set; }

    [Required]
    public required string Name { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public decimal? Limit { get; set; }
}
