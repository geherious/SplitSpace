using SplitSpace.ExternalFacade.Common.Models;
using SplitSpace.ExternalFacade.Logic.Models.Commands;
using SplitSpace.ExternalFacade.Logic.Models.Results;

namespace SplitSpace.ExternalFacade.Logic.Services;

public interface ITokenService
{
    Task<Result<AuthorizeUserResultData>> AuthorizeUserAsync(AuthorizeUserCommand command);
}