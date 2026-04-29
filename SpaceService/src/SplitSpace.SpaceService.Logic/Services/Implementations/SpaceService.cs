using SplitSpace.SpaceService.Common.Enums;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Dal.Models.Entities;
using SplitSpace.SpaceService.Dal.Repositories;
using SplitSpace.SpaceService.Logic.Models.Commands;
using SplitSpace.SpaceService.Logic.Models.Results;

namespace SplitSpace.SpaceService.Logic.Services.Implementations;

public class SpaceService : ISpaceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpaceRepository _spaceRepository;
    private readonly ISpaceMembershipRepository _spaceMembershipRepository;

    public SpaceService(ISpaceRepository spaceRepository,
        IUnitOfWork unitOfWork,
        ISpaceMembershipRepository spaceMembershipRepository)
    {
        _spaceRepository = spaceRepository;
        _unitOfWork = unitOfWork;
        _spaceMembershipRepository = spaceMembershipRepository;
    }

    public async Task<Result<GetSpacesResultData>> GetSpacesAsync(GetSpacesCommand command)
    {
        if (Guid.TryParse(command.UserId, out var userId) is false)
        {
            return Result<GetSpacesResultData>.Failure(new Error
            {
                Type = ErrorType.Validation,
                Message = "Invalid guid"
            });
        }
        
        var spaces = await _spaceRepository.GetByMembershipAsync(userId);

        var data = new List<GetSpacesResultData.Space>();
        foreach (var space in spaces)
        {
            var userRole = space.OwnerId == userId ? SpaceMembershipRole.Owner : SpaceMembershipRole.Member;
            data.Add(new GetSpacesResultData.Space(space.Id, space.Name, space.Type, userRole));
        }

        return Result<GetSpacesResultData>.Success(new GetSpacesResultData(data));
    }

    public async Task<Result<CreateSpaceResultData>> CreateSpaceAsync(CreateSpaceCommand command)
    {
        if (Guid.TryParse(command.UserId, out var userId) is false)
        {
            return Result<CreateSpaceResultData>.Failure(new Error
            {
                Type = ErrorType.Validation,
                Message = "Invalid guid"
            });
        }
        var currentTime = DateTime.UtcNow;
        var space = new Space
        {
            Id = Guid.CreateVersion7(),
            Name = command.Name,
            Type = command.Type,
            OwnerId = userId,
            CreatedAt = currentTime
        };

        var spaceMembership = new SpaceMembership
        {
            Id = Guid.CreateVersion7(),
            SpaceId = space.Id,
            UserId = userId,
            Role = SpaceMembershipRole.Owner,
            JoinedAt = DateTime.UtcNow,
        };

        await _spaceRepository.CreateSpaceAsync(space);
        await _spaceMembershipRepository.AddAsync(spaceMembership);
        await _unitOfWork.SaveChangesAsync();

        return Result<CreateSpaceResultData>.Success(new CreateSpaceResultData(space.Id));
    }

    public async Task<Result<DeleteSpaceResultData>> DeleteSpaceAsync(DeleteSpaceCommand command)
    {
        if (Guid.TryParse(command.UserId, out var userId) is false ||
            Guid.TryParse(command.SpaceId, out var spaceId) is false)
        {
            return Result<DeleteSpaceResultData>.Failure(new Error
            {
                Type = ErrorType.Validation,
                Message = "Invalid guid"
            });
        }
        var space = await _spaceRepository.GetAsync(userId, spaceId);
        
        if (space == null)
            return Result<DeleteSpaceResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Space not found"
            });

        var deletedCount = await _spaceRepository.DeleteSpaceAsync(spaceId);
        await _spaceMembershipRepository.DeleteAsync(spaceId);
        await _unitOfWork.SaveChangesAsync();
        var success = deletedCount > 0;

        return Result<DeleteSpaceResultData>.Success(new DeleteSpaceResultData(success));
    }
}
