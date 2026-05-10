using Google.Type;
using Riok.Mapperly.Abstractions;
using SplitSpace.ExternalFacade.Logic.Models.Requests.Space;
using SplitSpace.ExternalFacade.Logic.Models.Responses.Finance;
using SplitSpace.ExternalFacade.Logic.Models.Responses.Space;
using SplitSpace.FinanceService.Api.FinanceService;
using SplitSpace.FinanceService.Api.SearchService;
using SplitSpace.SpaceService.Api;

namespace SplitSpace.ExternalFacade.Mappers;

[Mapper(
    RequiredMappingStrategy = RequiredMappingStrategy.Target,
    EnumMappingStrategy = EnumMappingStrategy.ByName,
    RequiredEnumMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class FromGrpcMapper
{
    public static partial GetSpacesApiResponse ToApiResponse(this GetSpacesResponse grpcResponse);
    
    public static partial CreateSpaceApiResponse ToApiResponse(this CreateSpaceResponse grpcResponse);

    public static partial GetCategoriesApiResponse ToApiResponse(this GetCategoriesResponse grpcResponse);
    
    public static partial GetTagsApiResponse ToApiResponse(this GetTagsResponse grpcResponse);
    
    public static partial GetSpaceAccountsApiResponse ToApiResponse(this GetSpaceAccountsResponse grpcResponse);
    
    public static partial GetPersonalAccountsApiResponse ToApiResponse(this GetPersonalAccountsResponse grpcResponse);
    
    public static partial GetExpensesApiResponse ToApiResponse(this GetExpensesResponse grpcResponse);
    
    public static partial GetDebtsApiResponse ToApiResponse(this GetDebtsResponse grpcResponse);
    
    public static partial GetCategoryReportApiResponse ToApiResponse(this GetReportResponse grpcResponse);
    
    private static decimal? ToNullableMoney(Money? amount)
    {
        if (amount is null)
        {
            return null;
        }
        
        return amount.DecimalValue;
    }
    
    private static decimal ToMoney(Money amount)
    {
        return amount.DecimalValue;
    }
}
