using Microsoft.EntityFrameworkCore;
using Npgsql;
using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal.Repositories.Implementations;

public class DebtRepository : IDebtRepository
{
    private readonly FinanceServiceDbContext _dbContext;

    public DebtRepository(FinanceServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddOrUpdateAsync(IReadOnlyCollection<Debt> debts)
    {
        var sql =
            """
            WITH input AS (
                SELECT *
                FROM UNNEST(
                    @ids::uuid[],
                    @space_ids::uuid[],
                    @from_user_ids::uuid[],
                    @to_user_ids::uuid[],
                    @amounts::numeric[]
                ) AS t(id, space_id, from_user_id, to_user_id, amount)
            )
            INSERT INTO debt (id, space_id, from_user_id, to_user_id, amount)
            SELECT id, space_id, from_user_id, to_user_id, amount
            FROM input
            ON CONFLICT (space_id, from_user_id, to_user_id) DO UPDATE SET
                amount = debt.amount + EXCLUDED.amount;
            """;
        await _dbContext.Database.ExecuteSqlRawAsync(
            sql,
            new NpgsqlParameter<Guid[]>("ids", debts.Select(d => d.Id).ToArray()),
            new NpgsqlParameter<Guid[]>("space_ids", debts.Select(d => d.SpaceId).ToArray()),
            new NpgsqlParameter<Guid[]>("from_user_ids", debts.Select(d => d.FromUserId).ToArray()),
            new NpgsqlParameter<Guid[]>("to_user_ids", debts.Select(d => d.ToUserId).ToArray()),
            new NpgsqlParameter<decimal[]>("amounts", debts.Select(d => d.Amount).ToArray()));
    }

    public async Task<IReadOnlyCollection<Debt>> GetBatchAsync(Guid spaceId, Guid userId)
    {
        return await _dbContext.Debts
            .Where(d => d.SpaceId == spaceId && (d.FromUserId == userId || d.ToUserId == userId))
            .ToListAsync();
    }
}
