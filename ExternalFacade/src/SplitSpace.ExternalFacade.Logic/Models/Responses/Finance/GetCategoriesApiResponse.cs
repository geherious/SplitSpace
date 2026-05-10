namespace SplitSpace.ExternalFacade.Logic.Models.Responses.Finance;

public class GetCategoriesApiResponse
{
    public required List<GetCategoriesApiResponseCategory> Categories { get; set; }
}

public class GetCategoriesApiResponseCategory
{
    public required string CategoryId { get; set; }
    public required string Name { get; set; }
    public string? ParentCategoryId { get; set; }
    public decimal? Limit { get; set; }
}
