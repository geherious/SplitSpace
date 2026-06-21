using Grpc.Core;
using Mediator;
using SplitSpace.SpaceService.Api;
using SplitSpace.SpaceService.Api.Common;
using SplitSpace.SpaceService.Domain.Models.Ids;
using SplitSpace.SpaceService.Helpers;
using SplitSpace.SpaceService.Logic.Features.Spaces.CreateSpace;
using SplitSpace.SpaceService.Logic.Features.Spaces.DeleteSpace;
using SplitSpace.SpaceService.Logic.Features.Spaces.GetSpaces;
using SpaceDomain = SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

namespace SplitSpace.SpaceService.Services;

public class SpaceServiceGrpc : Api.SpaceService.SpaceServiceBase
{
    private readonly IMediator _mediator;

    public SpaceServiceGrpc(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetSpacesResponse> GetSpaces(GetSpacesRequest request, ServerCallContext context)
    {
        var command = new GetSpacesQuery(new UserId(Guid.Parse(request.UserId)));
        var result = await _mediator.Send(command,  context.CancellationToken);

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
                        SpaceDomain.SpaceType.Shared => SpaceType.Shared,
                        SpaceDomain.SpaceType.Private => SpaceType.Private,
                        _ => throw new ArgumentOutOfRangeException(nameof(SpaceType),
                            $"Unknown space type {s.SpaceType}")
                    },
                    UserRole = s.Role switch
                    {
                        SpaceDomain.SpaceMemberRole.Member => SpaceMembershipRole.Member,
                        SpaceDomain.SpaceMemberRole.Owner => SpaceMembershipRole.Owner,
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
            SpaceType.Private => SpaceDomain.SpaceType.Private,
            SpaceType.Shared => SpaceDomain.SpaceType.Shared,
            _ => throw new ArgumentOutOfRangeException(nameof(request.Type), "Unknown type")
        };
        
        var command = new CreateSpaceCommand(
            new UserId(Guid.Parse(request.UserId)),
            request.Name,
            spaceType);
        
        var result = await _mediator.Send(command, context.CancellationToken);

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
        var command = new DeleteSpaceCommand(
            new UserId(Guid.Parse(request.UserId)),
            new SpaceId(Guid.Parse(request.SpaceId)));
        
        var result = await _mediator.Send(command, context.CancellationToken);

        if (!result.IsSuccess)
        {
            throw ErrorMapper.ToRpcException(result.Errors[0]);
        }

        return new DeleteSpaceResponse();
    }
}
