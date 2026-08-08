using Mediator;
using SplitSpace.Finances.Dal.ClientFacades;
using SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Categories.GetCategories;

public class GetCategoriesHandler : IQueryHandler<GetCategoriesCommand, Result<GetCategoriesResultData>>
{
    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly ISpacesServiceClientFacade _spacesServiceClientFacade;

    public GetCategoriesHandler(ICategoryReadRepository categoryReadRepository, ISpacesServiceClientFacade spacesServiceClientFacade)
    {
        _categoryReadRepository = categoryReadRepository;
        _spacesServiceClientFacade = spacesServiceClientFacade;
    }

    public async ValueTask<Result<GetCategoriesResultData>> Handle(GetCategoriesCommand command, CancellationToken ct)
    {
        var spaceIds = await _spacesServiceClientFacade.GetUserSpaceIdsAsync(command.UserId, ct);
        if (spaceIds.Contains(command.SpaceId) is false)
        {
            return Result<GetCategoriesResultData>.Failure(new Error(ErrorType.NotFound, "Space not found"));
        }

        var categories = await _categoryReadRepository.GetBatchAsync(command.SpaceId, ct);

        var result = categories
            .Select(c => new GetCategoriesResultData.Category
            {
                Id = c.Id.Value,
                Name = c.Name,
                ParentId = c.ParentId,
                Limit = c.Limit
            })
            .ToArray();

        return Result<GetCategoriesResultData>.Success(new GetCategoriesResultData(result));
    }
}
