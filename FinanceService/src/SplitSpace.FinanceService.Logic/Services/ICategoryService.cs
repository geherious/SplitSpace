using SplitSpace.FinanceService.Common.Models;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Models.Results;

namespace SplitSpace.FinanceService.Logic.Services;

public interface ICategoryService
{
    Task<Result<AddCategoryResultData>> AddCategoryAsync(AddCategoryCommand command);
    
    Task<Result<GetCategoriesResultData>> GetCategoriesAsync(GetCategoriesCommand command);
}
