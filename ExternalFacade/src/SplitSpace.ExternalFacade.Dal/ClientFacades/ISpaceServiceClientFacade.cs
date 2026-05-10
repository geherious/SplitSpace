using SplitSpace.SpaceService.Api;

namespace SplitSpace.ExternalFacade.Dal.ClientFacades;

public interface ISpaceServiceClientFacade
{
    Task<GetSpacesResponse> GetSpacesAsync(GetSpacesRequest request, CancellationToken cancellationToken);
    
    Task<CreateSpaceResponse> CreateSpaceAsync(CreateSpaceRequest request, CancellationToken cancellationToken);

    Task<DeleteSpaceResponse> DeleteSpaceAsync(DeleteSpaceRequest request, CancellationToken cancellationToken);
}