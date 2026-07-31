using Mediator;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;

namespace SplitSpace.Spaces.Logic.Features.Spaces.DeleteSpace;

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
            return Result.Failure(new Error(ErrorType.NotFound, $"Space with id {command.SpaceId.Value} not found"));
        }

        if (space.DeleteBy(command.UserId) is false)
        {
            return Result.Failure(new Error(ErrorType.FailedPrecondition, $"Space with id {command.SpaceId.Value} can not be deleted by this user"));
        }
        
        await _spaceDomainRepository.SaveAsync(space, ct);

        return Result.Success();
    }
}
