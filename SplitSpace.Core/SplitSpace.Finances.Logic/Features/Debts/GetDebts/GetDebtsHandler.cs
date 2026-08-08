using Mediator;
using SplitSpace.Finances.Dal.ClientFacades;
using SplitSpace.Finances.Dal.Database.Repositories;
using SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Debts.GetDebts;

public class GetDebtsHandler : IQueryHandler<GetDebtsCommand, Result<GetDebtsResultData>>
{
    private readonly IDebtReadRepository _debtReadRepository;
    private readonly ISpacesServiceClientFacade _spacesServiceClientFacade;

    public GetDebtsHandler(IDebtReadRepository debtReadRepository, ISpacesServiceClientFacade spacesServiceClientFacade)
    {
        _debtReadRepository = debtReadRepository;
        _spacesServiceClientFacade = spacesServiceClientFacade;
    }

    public async ValueTask<Result<GetDebtsResultData>> Handle(GetDebtsCommand command, CancellationToken ct)
    {
        var spaceIds = await _spacesServiceClientFacade.GetUserSpaceIdsAsync(new UserId(command.UserId), ct);
        if (spaceIds.Contains(new SpaceId(command.SpaceId)) is false)
        {
            return Result<GetDebtsResultData>.Failure(new Error(ErrorType.NotFound, "Space not found"));
        }

        var debts = await _debtReadRepository.GetBatchAsync(command.SpaceId, command.UserId, ct);

        var result = debts
            .Select(d => new GetDebtsResultData.Debt(d.Id, d.SpaceId, d.FromUserId, d.ToUserId, d.Amount))
            .ToArray();

        return Result<GetDebtsResultData>.Success(new GetDebtsResultData(result));
    }
}
