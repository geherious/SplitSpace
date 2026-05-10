namespace SplitSpace.ExternalFacade.Logic.Models.Responses.Finance;

public class GetTagsApiResponse
{
    public required List<GetTagsApiResponseTag> Tags { get; set; }
}

public class GetTagsApiResponseTag
{
    public required string TagId { get; set; }
    public required string Name { get; set; }
}