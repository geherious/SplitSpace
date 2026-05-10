using SplitSpace.FinanceService.Api.FinanceService;
using SplitSpace.FinanceService.Common.Exceptions;
using SplitSpace.FinanceService.Logic.Models.Commands;

namespace SplitSpace.FinanceService.Mappers;

public static class FromGrpcMapper
{
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