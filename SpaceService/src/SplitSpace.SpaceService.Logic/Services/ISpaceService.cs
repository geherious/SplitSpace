using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Logic.Models.Commands;
using SplitSpace.SpaceService.Logic.Models.Results;

namespace SplitSpace.SpaceService.Logic.Services;

public interface ISpaceService
{
    Task<Result<GetSpacesResultData>> GetSpacesAsync(GetSpacesCommand command);
    Task<Result<CreateSpaceResultData>> CreateSpaceAsync(CreateSpaceCommand command);
    Task<Result<DeleteSpaceResultData>> DeleteSpaceAsync(DeleteSpaceCommand command);
}
