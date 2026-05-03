using SplitSpace.FinanceService.Common.Models;
using SplitSpace.FinanceService.Dal.Models.Entities;
using SplitSpace.FinanceService.Dal.Repositories;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Models.Results;
using SplitSpace.SpaceService.Dal.Repositories;

namespace SplitSpace.FinanceService.Logic.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISpaceMembershipRepository _spaceMembershipRepository;

    public CategoryService(ICategoryRepository categoryRepository,
        ISpaceMembershipRepository spaceMembershipRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _spaceMembershipRepository = spaceMembershipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddCategoryResultData>> AddCategoryAsync(AddCategoryCommand command)
    {
        var membership = await _spaceMembershipRepository.GetAsync(command.SpaceId, command.UserId);

        if (membership is null)
        {
            return Result<AddCategoryResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Space not found"
            });
        }

        if (command.ParentId is not null)
        {
            var parent = await _categoryRepository.GetAsync(command.ParentId.Value);

            if (parent is null)
            {
                return Result<AddCategoryResultData>.Failure(new Error
                {
                    Type = ErrorType.FailedPrecondition,
                    Message = "Parent category doesn't exist."
                });
            }

            if (parent.ParentId is not null)
            {
                return Result<AddCategoryResultData>.Failure(new Error
                {
                    Type = ErrorType.FailedPrecondition,
                    Message = "Parent category already has parent."
                });
            }
        }

        var category = new Category
        {
            Id = Guid.CreateVersion7(),
            SpaceId = command.SpaceId,
            Name = command.Name,
            ParentId = command.ParentId,
            Limit = command.Limit
        };

        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return Result<AddCategoryResultData>.Success(new AddCategoryResultData(category.Id));
    }

    public async Task<Result<GetCategoriesResultData>> GetCategoriesAsync(GetCategoriesCommand command)
    {
        var membership = await _spaceMembershipRepository.GetAsync(command.SpaceId, command.UserId);

        if (membership is null)
        {
            return Result<GetCategoriesResultData>.Failure(new Error
            {
                Type = ErrorType.NotFound,
                Message = "Space not found"
            });
        }

        var categories = await _categoryRepository.GetBatchAsync(command.SpaceId);
        var mappedCategories = categories
            .Select(c => new GetCategoriesResultData.Category
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentId,
                Limit = c.Limit
            })
            .ToArray();

        return Result<GetCategoriesResultData>.Success(new GetCategoriesResultData(mappedCategories));
    }
}
