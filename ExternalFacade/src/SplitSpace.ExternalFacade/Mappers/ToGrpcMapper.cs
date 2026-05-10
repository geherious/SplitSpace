using Google.Protobuf.WellKnownTypes;
using Google.Type;
using Riok.Mapperly.Abstractions;
using SplitSpace.ExternalFacade.Logic.Models.Requests.Finance;
using SplitSpace.ExternalFacade.Logic.Models.Requests.Space;
using SplitSpace.FinanceService.Api.FinanceService;
using SplitSpace.SpaceService.Api;

namespace SplitSpace.ExternalFacade.Mappers;

[Mapper(
    RequiredMappingStrategy = RequiredMappingStrategy.Target,
    EnumMappingStrategy = EnumMappingStrategy.ByName,
    RequiredEnumMappingStrategy = RequiredMappingStrategy.Source)]
public static partial class ToGrpcMapper
{
    public static partial CreateSpaceRequest ToGrpcRequest(this CreateSpaceApiRequest request, Guid userId);
    
    public static partial InviteUserRequest ToGrpcRequest(this InviteUserApiRequest request, Guid invitedByUserId);

    public static partial AddCategoryRequest ToGrpcRequest(this AddCategoryApiRequest request, Guid userId);
    
    public static AddExpenseRequest ToGrpcRequest(this AddExpenseApiRequest request, Guid userId)
    {
        AddExpenseRequest.Types.Split? split = null;
        if (request.Split is not null)
        {
            split = new AddExpenseRequest.Types.Split();
            split.TemplateSplit = new AddExpenseRequest.Types.TemplateSplit
            {
                Template = request.Split.Template!.Value.ToTemplate()
            };
        }

        return new AddExpenseRequest
        {
            SpaceId = request.SpaceId.ToString(),
            UserId = userId.ToString(),
            CategoryId = request.CategoryId.ToString(),
            AccountId = request.AccountId.ToString(),
            Amount = ToMoney(request.Amount),
            Description = request.Description,
            Split = split,
            CreatedAt = request.CreatedAt.ToUniversalTime().ToTimestamp()
        };
    }
    
    public static partial SettleDebtRequest ToGrpcRequest(this SettleDebtApiRequest request, Guid userId);
    
    private static partial AddExpenseRequest.Types.AddExpenseSplitTemplate ToTemplate(
        this AddExpenseApiRequestTemplateType type);

    private static Money ToMoney(decimal amount)
    {
        return new Money { DecimalValue = amount };
    }
}
