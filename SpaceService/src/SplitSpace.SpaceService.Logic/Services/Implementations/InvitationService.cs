using SplitSpace.SpaceService.Common.Enums;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Dal.ClientFacades.Implementations;
using SplitSpace.SpaceService.Dal.Models.Entities;
using SplitSpace.SpaceService.Dal.Repositories;
using SplitSpace.SpaceService.Logic.Models.Commands;
using SplitSpace.SpaceService.Logic.Models.Results;
using SplitSpace.SpaceService.Producing.Models.SpaceEvents;
using SplitSpace.SpaceService.Producing.Producers;

namespace SplitSpace.SpaceService.Logic.Services.Implementations;

public class InvitationService : IInvitationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvitationRepository _invitationRepository;
    private readonly ISpaceRepository _spaceRepository;
    private readonly ISpaceMembershipRepository _spaceMembershipRepository;
    private readonly IAuthServiceClientFacade _authServiceClientFacade;
    private readonly ISpaceEventsProducer _spaceEventsProducer;

    public InvitationService(IInvitationRepository invitationRepository,
        ISpaceRepository spaceRepository,
        IAuthServiceClientFacade authServiceClientFacade,
        IUnitOfWork unitOfWork,
        ISpaceMembershipRepository spaceMembershipRepository,
        ISpaceEventsProducer spaceEventsProducer)
    {
        _invitationRepository = invitationRepository;
        _spaceRepository = spaceRepository;
        _authServiceClientFacade = authServiceClientFacade;
        _unitOfWork = unitOfWork;
        _spaceMembershipRepository = spaceMembershipRepository;
        _spaceEventsProducer = spaceEventsProducer;
    }

    public async Task<Result<InviteUserResultData>> InviteUserAsync(InviteUserCommand command)
    {
        if (Guid.TryParse(command.InvitedByUserId, out var invitedByUserId) is false ||
            Guid.TryParse(command.SpaceId, out var spaceId) is false)
        {
            return Result<InviteUserResultData>.Failure(new Error
            {
                Type = ErrorType.Validation,
                Message = "Invalid guid"
            });
        }
        var invitedUserId = await _authServiceClientFacade.UserExistAsync(command.InvitedUserEmail);

        if (invitedUserId == null)
        {
            return Result<InviteUserResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = $"User {command.InvitedUserEmail} not found."
            });
        }

        if (invitedUserId == invitedByUserId)
        {
            return Result<InviteUserResultData>.Failure(new Error
            {
                Type = ErrorType.FailedPrecondition,
                Message = $"Cannot invite yourself."
            });
        }

        var space = await _spaceRepository.GetAsync(invitedByUserId, spaceId);

        if (space == null)
        {
            return Result<InviteUserResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = $"Space not found."
            });
        }
        
        var membership = await _spaceMembershipRepository.GetAsync(spaceId, invitedUserId.Value);

        if (membership is not null)
        {
            return Result<InviteUserResultData>.Failure(new Error
            {
                Type = ErrorType.FailedPrecondition,
                Message = $"User is already a space member."
            });
        }
        
        var existingInvitation = await _invitationRepository.GetAsync(
            spaceId,
            invitedUserId.Value,
            InvitationStatus.Pending);

        if (existingInvitation is not null)
        {
            return Result<InviteUserResultData>.Failure(new Error
            {
                Type = ErrorType.FailedPrecondition,
                Message = $"User is already invited."
            });
        }

        var expiresAt = DateTime.UtcNow.AddDays(7);

        var invitation = new Invitation
        {
            Id = Guid.CreateVersion7(),
            SpaceId = spaceId,
            InvitedUserId = invitedUserId.Value,
            InvitedBy = invitedByUserId,
            Status = InvitationStatus.Pending,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };

        await _invitationRepository.AddAsync(invitation);
        await _unitOfWork.SaveChangesAsync();

        return Result<InviteUserResultData>.Success(new InviteUserResultData(invitation.Id));
    }

    public async Task<Result> AcceptInvitationAsync(AcceptInvitationCommand command)
    {
        if (Guid.TryParse(command.InvitationId, out var invitationId) is false ||
            Guid.TryParse(command.UserId, out var userId) is false)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.Validation,
                Message = "Invalid guid"
            });
        }
        var invitation = await _invitationRepository.GetAsync(invitationId);

        if (invitation == null || invitation.InvitedUserId != userId)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = $"Invitation not found."
            });
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.FailedPrecondition,
                Message = $"Invitation can not be accepted."
            });
        }

        var membership = new SpaceMembership
        {
            Id = Guid.CreateVersion7(),
            SpaceId = invitation.SpaceId,
            UserId = userId,
            Role = SpaceMembershipRole.Member,
            JoinedAt = DateTime.UtcNow
        };
        
        using var transaction = _unitOfWork.BeginTransactionAsync();
        await _invitationRepository.AcceptInvitationAsync(invitationId);
        await _spaceMembershipRepository.AddAsync(membership);
        
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();
        
        var spaceEvent = new SpaceEvent<UsersAddedToSpaceEventData>
        {
            SpaceId = invitation.SpaceId,
            Data = new UsersAddedToSpaceEventData
            {
                SpaceId = invitation.SpaceId,
                UserIds = [invitation.InvitedUserId]
            }
        };
        await _spaceEventsProducer.ProduceAsync(spaceEvent);
        
        return Result.Success();
    }

    public async Task<Result> RejectInvitationAsync(RejectInvitationCommand command)
    {
        if (Guid.TryParse(command.InvitationId, out var invitationId) is false ||
            Guid.TryParse(command.UserId, out var userId) is false)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.Validation,
                Message = "Invalid guid"
            });
        }
        var invitation = await _invitationRepository.GetAsync(invitationId);

        if (invitation == null || invitation.InvitedUserId != userId)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = $"Invitation not found."
            });
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.FailedPrecondition,
                Message = $"Invitation can not be accepted."
            });
        }
        
        await _invitationRepository.RejectInvitationAsync(invitationId);
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }
}
