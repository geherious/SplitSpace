using Mediator;
using SplitSpace.Finances.Dal.ClientFacades;
using SplitSpace.Finances.Dal.Database.Transactions;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Categories.AddCategory;

public class AddCategoryHandler : ICommandHandler<AddCategoryCommand, Result<AddCategoryResultData>>
{
    private readonly ITransactionProvider _transactionProvider;
    private readonly ISpacesServiceClientFacade _spacesServiceClientFacade;
    private readonly ICategoryDomainRepository _categoryDomainRepository;

    public AddCategoryHandler(ITransactionProvider transactionProvider, ISpacesServiceClientFacade spacesServiceClientFacade, ICategoryDomainRepository categoryDomainRepository)
    {
        _transactionProvider = transactionProvider;
        _spacesServiceClientFacade = spacesServiceClientFacade;
        _categoryDomainRepository = categoryDomainRepository;
    }

    public async ValueTask<Result<AddCategoryResultData>> Handle(AddCategoryCommand command, CancellationToken ct)
    {
        var spaceIds = await _spacesServiceClientFacade.GetUserSpaceIdsAsync(command.UserId, ct);
        if (spaceIds.Contains(command.SpaceId) is false)
        {
            return Result<AddCategoryResultData>.Failure(new Error(ErrorType.NotFound, "Space not found"));
        }

        Category? parent = null;
        if (command.ParentId is not null)
        {
            parent = await _categoryDomainRepository.GetAsync(command.ParentId.Value, ct);

            if (parent is null)
            {
                return Result<AddCategoryResultData>.Failure(new Error(ErrorType.NotFound, "Parent category doesn't exist."));
            }
        }

        var createdCategory = Category.Create(command.SpaceId, command.Name, parent, command.Limit);
        if (createdCategory.IsSuccess is false)
        {
            return Result<AddCategoryResultData>.Failure(createdCategory.Errors);
        }
        await _categoryDomainRepository.SaveAsync(createdCategory.ResultValue, ct);

        return Result<AddCategoryResultData>.Success(new AddCategoryResultData(createdCategory.ResultValue.Id));
    }
}
