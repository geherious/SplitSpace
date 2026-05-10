using SplitSpace.ExternalFacade.Logic.Models.Enums;

namespace SplitSpace.ExternalFacade.Logic.Models.Requests.Space;

public class CreateSpaceApiRequest
{
    public required string Name { get; set; }
    
    public required SpaceType Type { get; set; }
}
