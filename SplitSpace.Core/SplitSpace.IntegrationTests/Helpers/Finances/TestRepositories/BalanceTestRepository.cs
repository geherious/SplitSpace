using Dapper;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;
using SplitSpace.Finances.Domain.Models.Aggregates.BalanceAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.IntegrationTests.Helpers.Finances.Models;

namespace SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;

public sealed class BalanceTestRepository
{
    private readonly IBalanceDomainRepository _domainRepository;
    private readonly IBalanceReadRepository _readRepository;
    private readonly IFinanceDbConnectionFactory _connectionFactory;

    public BalanceTestRepository(
        IBalanceDomainRepository domainRepository,
        IBalanceReadRepository readRepository,
        IFinanceDbConnectionFactory connectionFactory)
    {
        _domainRepository = domainRepository;
        _readRepository = readRepository;
        _connectionFactory = connectionFactory;
    }

    public Task SaveAsync(Balance balance, CancellationToken ct = default)
    {
        return _domainRepository.SaveAsync(balance, ct);
    }

    public Task<Balance?> GetAsync(BalanceId balanceId, CancellationToken ct = default)
    {
        return _domainRepository.GetAsync(balanceId, ct);
    }

    public Task<IReadOnlyCollection<BalanceEntity>> GetSpaceBalanceBatchAsync(SpaceId spaceId, CancellationToken ct = default)
    {
        return _readRepository.GetSpaceBalanceBatchAsync(spaceId, ct);
    }

    public Task<IReadOnlyCollection<BalanceEntity>> GetUserBalanceBatchAsync(UserId userId, CancellationToken ct = default)
    {
        return _readRepository.GetUserBalanceBatchAsync(userId, ct);
    }

    public async Task<IReadOnlyCollection<BalanceEntryEntity>> ReadEntriesAsync(BalanceId balanceId)
    {
        const string sql =
            """
            SELECT
                id,
                balance_id AS BalanceId,
                total,
                source_type AS SourceType,
                expense_id AS ExpenseId
            FROM balance_entry
            WHERE balance_id = @BalanceId
            """;

        var parameters = new
        {
            BalanceId = balanceId.Value
        };

        await using var connection = await _connectionFactory.CreateAsync(CancellationToken.None);
        var result = (await connection.QueryAsync<BalanceEntryEntity>(sql, parameters)).ToArray();
        return result;
    }
}
