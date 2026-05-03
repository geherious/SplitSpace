using SplitSpace.FinanceService.Common.Models;
using SplitSpace.FinanceService.Logic.Models.Commands;
using SplitSpace.FinanceService.Logic.Models.Results;

namespace SplitSpace.FinanceService.Logic.Services;

public interface ITagService
{
    Task<Result<AddTagResultData>> AddTagAsync(AddTagCommand command);
    
    Task<Result<GetTagsResultData>> GetTagsAsync(GetTagsCommand command);
}
