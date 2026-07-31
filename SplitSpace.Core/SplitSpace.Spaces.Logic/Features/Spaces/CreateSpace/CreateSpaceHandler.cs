using Mediator;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate;
using SpaceAggregate = SplitSpace.Spaces.Domain.Models.Aggregates.SpaceAggregate.Space;

namespace SplitSpace.Spaces.Logic.Features.Spaces.CreateSpace;

public class CreateSpaceHandler : ICommandHandler<CreateSpaceCommand, Result<CreateSpaceResultData>>
{
    private readonly TimeProvider _timeProvider;
    
    private readonly ISpaceDomainRepository _spaceDomainRepository;

    public CreateSpaceHandler(
        TimeProvider timeProvider,
        ISpaceDomainRepository spaceDomainRepository)
    {
        _timeProvider = timeProvider;
        _spaceDomainRepository = spaceDomainRepository;
    }

    public async ValueTask<Result<CreateSpaceResultData>> Handle(CreateSpaceCommand command, CancellationToken ct)
    {
        var currentTime = _timeProvider.GetUtcNow();

        var space = command.Type switch
        {
            SpaceType.Private => SpaceAggregate.CreatePrivate(command.Name, command.UserId, currentTime),
            SpaceType.Shared => SpaceAggregate.CreateShared(command.Name, command.UserId, [], currentTime),
            _ => throw new ArgumentOutOfRangeException(nameof(SpaceType))
        };

        await _spaceDomainRepository.SaveAsync(space, ct);

        return Result<CreateSpaceResultData>.Success(new CreateSpaceResultData(space.Id));
    }
}
