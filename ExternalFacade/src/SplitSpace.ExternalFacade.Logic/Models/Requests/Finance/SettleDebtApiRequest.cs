using System.ComponentModel.DataAnnotations;
using Google.Protobuf.WellKnownTypes;

namespace SplitSpace.ExternalFacade.Logic.Models.Requests.Finance;

public class SettleDebtApiRequest
{
    [Required]
    public required Guid SpaceId { get; set; }

    public Guid? AccountId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public required Guid UserTo { get; set; }
}
