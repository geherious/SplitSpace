using Mediator;
using SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;
using SplitSpace.Finances.Domain.Models.Events;

namespace SplitSpace.Finances.Logic.Events.ExpenseCreated;

public class ExpenseCreatedDebtHandler : INotificationHandler<ExpenseCreatedEvent>
{
    private readonly IDebtDomainRepository _debtDomainRepository;

    public ExpenseCreatedDebtHandler(IDebtDomainRepository debtDomainRepository)
    {
        _debtDomainRepository = debtDomainRepository;
    }

    public async ValueTask Handle(ExpenseCreatedEvent notification, CancellationToken cancellationToken)
    {
        var expense = notification.Expense;
        var splits = notification.Expense.ExpenseSplits;
        foreach (var split in splits)
        {
            if (split.UserId == expense.CreatedBy)
                continue;
            
            var debtToCreate = Debt.Create(expense.SpaceId, split.UserId, expense.CreatedBy, split.AmountToPay);
            var existingDebt = await _debtDomainRepository.GetOrCreate(debtToCreate, cancellationToken);
            
            var debtEntry = DebtEntry.Create(
                existingDebt.Id,
                ownedBy: split.UserId,
                ownedTo: expense.CreatedBy,
                split.AmountToPay,
                new DebtEntry.DebtEntrySource.ExpenseSplit(expense.Id, split.Id));

            existingDebt.ApplyEntry(debtEntry);

            await _debtDomainRepository.SaveAsync(existingDebt, cancellationToken);
        }
    }
}
