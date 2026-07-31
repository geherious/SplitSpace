using Mediator;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Events;

namespace SplitSpace.Finances.Logic.Events.ExpenseCreated;

public class ExpenseCreatedBalanceHandler : INotificationHandler<ExpenseCreatedEvent>
{
    private readonly IBalanceDomainRepository _balanceDomainRepository;

    public ExpenseCreatedBalanceHandler(IBalanceDomainRepository balanceDomainRepository)
    {
        _balanceDomainRepository = balanceDomainRepository;
    }

    public async ValueTask Handle(ExpenseCreatedEvent notification, CancellationToken cancellationToken)
    {
        var balance = await _balanceDomainRepository.GetAsync(notification.Expense.BalanceId, cancellationToken);

        if (balance == null)
        {
            throw new ArgumentException($"Balance with id {notification.Expense.BalanceId} does not exist");
        }

        var balanceEntry = BalanceEntry.Create(
            balance.Id,
            balance.Total + notification.Expense.Amount,
            new BalanceEntry.BalanceEntrySource.Expense(notification.Expense.Id));
        
        balance.ApplyEntry(balanceEntry);
        await _balanceDomainRepository.SaveAsync(balance, cancellationToken);
    }
}
