using SplitSpace.FinanceService.Api.Common;
using SplitSpace.FinanceService.Api.FinanceService;
using SplitSpace.FinanceService.Common.Exceptions;
using SplitSpace.FinanceService.Logic.Models.Commands;
using DomainEnums = SplitSpace.FinanceService.Common.Enums;

namespace SplitSpace.FinanceService.Mappers;

public static class FromGrpcMapper
{
    public static DomainEnums.AccountOwnerType ToDomain(this AccountOwnerType accountOwnerType)
    {
        return accountOwnerType switch
        {
            AccountOwnerType.Space => DomainEnums.AccountOwnerType.Space,
            AccountOwnerType.Personal => DomainEnums.AccountOwnerType.Personal,
            _ => throw new ValidationException($"Invalid {nameof(AccountOwnerType)}")
        };
    }

    public static AddExpenseCommand.ExpenseSplit ToCommand(
        this AddExpenseRequest.Types.TemplateSplit templateSplit)
    {
        return new AddExpenseCommand.ExpenseSplit.Template(templateSplit.Template.ToCommandEnum());
    }
    
    private static AddExpenseCommand.ExpenseSplitTemplate ToCommandEnum(
        this AddExpenseRequest.Types.AddExpenseSplitTemplate template)
    {
        return template switch
        {
            AddExpenseRequest.Types.AddExpenseSplitTemplate.Equal => AddExpenseCommand.ExpenseSplitTemplate.Equal,
            _ => throw new ValidationException($"Invalid {nameof(AddExpenseRequest.Types.AddExpenseSplitTemplate)}")
        };
    }
}