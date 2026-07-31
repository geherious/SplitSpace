using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Auth.Domain.Services;

public interface IAccessTokenService
{
    string Generate(User user);
    
    Result<AccessTokenValidationResultData> Validate(string token);
}
