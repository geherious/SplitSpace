using SplitSpace.SpaceService.Api;

namespace SplitSpace.ExternalFacade.Dal.ClientFacades.Implementations;

public class SpaceServiceClientFacade : ISpaceServiceClientFacade
{
    private readonly SpaceService.Api.SpaceService.SpaceServiceClient _spaceServiceClient;

    public SpaceServiceClientFacade(SpaceService.Api.SpaceService.SpaceServiceClient spaceServiceClient)
    {
        _spaceServiceClient = spaceServiceClient;
    }

    public async Task<GetSpacesResponse> GetSpacesAsync(GetSpacesRequest request, CancellationToken cancellationToken)
    {
        var response = await _spaceServiceClient.GetSpacesAsync(request, cancellationToken: cancellationToken);
        
        return response;
    }

    public async Task<CreateSpaceResponse> CreateSpaceAsync(CreateSpaceRequest request, CancellationToken cancellationToken)
    {
        var response = await _spaceServiceClient.CreateSpaceAsync(request, cancellationToken: cancellationToken);
        
        return response;
    }

    public async Task<DeleteSpaceResponse> DeleteSpaceAsync(DeleteSpaceRequest request, CancellationToken cancellationToken)
    {
        var response = await _spaceServiceClient.DeleteSpaceAsync(request, cancellationToken: cancellationToken);
        
        return response;
    }
}