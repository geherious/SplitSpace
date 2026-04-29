using Grpc.Core;
using SplitSpace.SpaceService.Api;
using SplitSpace.SpaceService.Common.Enums;
using SplitSpace.SpaceService.Helpers;
using SplitSpace.SpaceService.Logic.Models.Commands;
using SplitSpace.SpaceService.Logic.Services;

namespace SplitSpace.SpaceService.Services;

public class SpaceServiceGrpc : Api.SpaceService.SpaceServiceBase
{
    private readonly ISpaceService _spaceService;

    public SpaceServiceGrpc(ISpaceService spaceService)
    {
        _spaceService = spaceService;
    }

    public override async Task<GetSpacesResponse> GetSpaces(GetSpacesRequest request, ServerCallContext context)
    {
        var result = await _spaceService.GetSpacesAsync(new GetSpacesCommand(request.UserId));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new GetSpacesResponse
        {
            Spaces =
            {
                result.Value.Spaces.Select(s => new GetSpacesResponse.Types.Space
                {
                    Name = s.Name,
                    SpaceId = s.Id.ToString(),
                    Type = s.SpaceType switch
                    {
                        SpaceType.Shared => Api.Common.SpaceType.Shared,
                        SpaceType.Private => Api.Common.SpaceType.Private,
                        _ => throw new ArgumentOutOfRangeException(nameof(SpaceType),
                            $"Unknown space type {s.SpaceType}")
                    },
                    UserRole = s.Role switch
                    {
                        SpaceMembershipRole.Member => Api.Common.SpaceMembershipRole.Member,
                        SpaceMembershipRole.Owner => Api.Common.SpaceMembershipRole.Owner,
                        _ => throw new ArgumentOutOfRangeException(nameof(SpaceMembershipRole),
                            $"Unknown role {s.Role}")
                    }
                })
            }
        };
    }

    public override async Task<CreateSpaceResponse> CreateSpace(CreateSpaceRequest request, ServerCallContext context)
    {
        var spaceType = request.Type switch
        {
            Api.Common.SpaceType.Private => SpaceType.Private,
            Api.Common.SpaceType.Shared => SpaceType.Shared,
            _ => throw new ArgumentOutOfRangeException(nameof(request.Type), "Unknown type")
        };
        
        var result = await _spaceService.CreateSpaceAsync(
            new CreateSpaceCommand(request.UserId, request.Name, spaceType));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new CreateSpaceResponse
        {
            SpaceId = result.Value.SpaceId.ToString()
        };
    }

    public override async Task<DeleteSpaceResponse> DeleteSpace(DeleteSpaceRequest request, ServerCallContext context)
    {
        var result = await _spaceService.DeleteSpaceAsync(new DeleteSpaceCommand(request.UserId, request.SpaceId));

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new DeleteSpaceResponse
        {
            Success = result.Value.Success
        };
    }
}
