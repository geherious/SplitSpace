using SplitSpace.ExternalFacade.Logic.Models.Enums;

namespace SplitSpace.ExternalFacade.Logic.Models.Responses.Space;

public class GetSpacesApiResponse
{
    public required List<GetSpacesApiResponseSpace> Spaces { get; set; }
}

public class GetSpacesApiResponseSpace
{
    public required string SpaceId { get; set; }
    
    public required string Name { get; set; }

    public required SpaceType Type { get; set; }
    
    public required SpaceMembershipRole UserRole { get; set; }
}
