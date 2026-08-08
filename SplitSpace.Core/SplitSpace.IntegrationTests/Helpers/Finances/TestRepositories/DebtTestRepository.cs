using Dapper;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;
using SplitSpace.Finances.Domain.Models.Aggregates.DebtAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.IntegrationTests.Helpers.Finances.Models;

namespace SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;

public sealed class DebtTestRepository
{
    private readonly IDebtDomainRepository _domainRepository;
    private readonly IDebtReadRepository _readRepository;
    private readonly IFinanceDbConnectionFactory _connectionFactory;

    public DebtTestRepository(
        IDebtDomainRepository domainRepository,
        IDebtReadRepository readRepository,
        IFinanceDbConnectionFactory connectionFactory)
    {
        _domainRepository = domainRepository;
        _readRepository = readRepository;
        _connectionFactory = connectionFactory;
    }

    public Task SaveAsync(Debt debt, CancellationToken ct = default)
    {
        return _domainRepository.SaveAsync(debt, ct);
    }

    public Task<Debt> GetOrCreate(Debt debt, CancellationToken ct = default)
    {
        return _domainRepository.GetOrCreate(debt, ct);
    }

    public Task<IReadOnlyCollection<DebtEntity>> GetBatchAsync(Guid spaceId, Guid userId, CancellationToken ct = default)
    {
        return _readRepository.GetBatchAsync(spaceId, userId, ct);
    }

    public async Task<IReadOnlyCollection<DebtEntryEntity>> ReadEntriesAsync(DebtId debtId)
    {
        const string sql =
            """
            SELECT
                id,
                debt_id AS DebtId,
                owned_by AS OwnedBy,
                owned_to AS OwnedTo,
                total,
                source_type AS SourceType,
                expense_id AS ExpenseId,
                split_id AS SplitId
            FROM debt_entry
            WHERE debt_id = @DebtId
            """;
        
        var parameters = new { DebtId = debtId.Value };

        await using var connection = await _connectionFactory.CreateAsync(CancellationToken.None);
        var result = (await connection.QueryAsync<DebtEntryEntity>(sql, parameters)).ToArray();
        return result;
    }
}
