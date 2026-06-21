using Mediator;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

namespace SplitSpace.SpaceService.Logic.Features.Spaces.DeleteSpace;

public class DeleteSpaceHandler : ICommandHandler<DeleteSpaceCommand, Result>
{
    private readonly ISpaceDomainRepository _spaceDomainRepository;

    public DeleteSpaceHandler(
        ISpaceDomainRepository spaceDomainRepository)
    {
        _spaceDomainRepository = spaceDomainRepository;
    }

    public async ValueTask<Result> Handle(DeleteSpaceCommand command, CancellationToken ct)
    {
        var space = await _spaceDomainRepository.GetAsync(command.SpaceId, ct);

        if (space is null)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Code = null,
                Message = $"Space with id {command.SpaceId.Value} not found"
            });
        }

        if (space.DeleteBy(command.UserId) is false)
        {
            return Result.Failure(new Error
            {
                Type = ErrorType.FailedPrecondition,
                Code = null,
                Message = $"Space with id {command.SpaceId.Value} can not be deleted by this user"
            });
        }
        
        await _spaceDomainRepository.SaveAsync(space, ct);

        return Result.Success();
    }
}
