using SplitSpace.FinanceService.Common.Models;
using SplitSpace.FinanceService.Dal.Models.Entities;
using SplitSpace.FinanceService.Dal.Repositories;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Models.Results;
using SplitSpace.SpaceService.Dal.Repositories;

namespace SplitSpace.FinanceService.Logic.Services.Implementations;

public class TagService : ITagService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITagRepository _tagRepository;
    private readonly ISpaceMembershipRepository _spaceMembershipRepository;

    public TagService(ITagRepository tagRepository,
        ISpaceMembershipRepository spaceMembershipRepository,
        IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _spaceMembershipRepository = spaceMembershipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddTagResultData>> AddTagAsync(AddTagCommand command)
    {
        var membership = await _spaceMembershipRepository.GetAsync(command.SpaceId, command.UserId);

        if (membership is null)
        {
            return Result<AddTagResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Space not found"
            });
        }

        var tag = new Tag
        {
            Id = Guid.CreateVersion7(),
            SpaceId = command.SpaceId,
            Name = command.Name
        };

        await _tagRepository.AddAsync(tag);
        await _unitOfWork.SaveChangesAsync();

        return Result<AddTagResultData>.Success(new AddTagResultData(tag.Id));
    }

    public async Task<Result<GetTagsResultData>> GetTagsAsync(GetTagsCommand command)
    {
        var membership = await _spaceMembershipRepository.GetAsync(command.SpaceId, command.UserId);

        if (membership is null)
        {
            return Result<GetTagsResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Space not found"
            });
        }

        var tags = await _tagRepository.GetBatchAsync(command.SpaceId);
        var mappedTags = tags
            .Select(t => new GetTagsResultData.Tag
            {
                Id = t.Id,
                Name = t.Name
            })
            .ToArray();

        return Result<GetTagsResultData>.Success(new GetTagsResultData(mappedTags));
    }
}
