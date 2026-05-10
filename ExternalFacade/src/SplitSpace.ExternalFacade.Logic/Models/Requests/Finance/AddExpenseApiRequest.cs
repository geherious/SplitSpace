using System.ComponentModel.DataAnnotations;
using Google.Protobuf.WellKnownTypes;

namespace SplitSpace.ExternalFacade.Logic.Models.Requests.Finance;

public class AddExpenseApiRequest
{
    [Required]
    public required Guid SpaceId { get; set; }

    [Required]
    public required Guid CategoryId { get; set; }

    [Required]
    public required Guid AccountId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public AddExpenseApiRequestSplit? Split { get; set; }
    
    [Required]
    public required DateTimeOffset CreatedAt { get; set; }
}

public enum AddExpenseApiRequestTemplateType
{
    Equal
}

public class AddExpenseApiRequestSplit
{
    public AddExpenseApiRequestTemplateType? Template { get; set; }
}
