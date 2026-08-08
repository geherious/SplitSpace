using Dapper;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Domain.Models.Aggregates.SettlementAggregate;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.Finances.Domain.Models.ValueObjects;
using SplitSpace.IntegrationTests.Helpers.Finances.Models;

namespace SplitSpace.IntegrationTests.Helpers.Finances.TestRepositories;

public sealed class SettlementTestRepository
{
    private readonly ISettlementDomainRepository _domainRepository;
    private readonly IFinanceDbConnectionFactory _connectionFactory;

    public SettlementTestRepository(
        ISettlementDomainRepository domainRepository,
        IFinanceDbConnectionFactory connectionFactory)
    {
        _domainRepository = domainRepository;
        _connectionFactory = connectionFactory;
    }

    public Task SaveAsync(Settlement settlement, CancellationToken ct = default)
    {
        return _domainRepository.SaveAsync(settlement, ct);
    }

    public async Task<SettlementEntity?> GetAsync(SettlementId settlementId, CancellationToken ct = default)
    {
        const string sql =
            """
            SELECT
                id,
                from_user_id AS FromUserId,
                from_balance_id AS FromBalanceId,
                to_user_id AS ToUserId,
                amount,
                created_at AS CreatedAt
            FROM settlement
            WHERE id = @Id
            """;

        await using var connection = await _connectionFactory.CreateAsync(ct);
        var entity = (await connection.QueryAsync<SettlementEntity>(
            sql,
            new { Id = settlementId.Value })).SingleOrDefault();
        return entity;
    }
}
