using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Dal.Database.Repositories.ReadRepositoriesAbstractions;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.Finances.Domain.Models.Ids;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class CategoryReadRepository : ICategoryReadRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;

    public CategoryReadRepository(IFinanceDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyCollection<Category>> GetBatchAsync(SpaceId spaceId, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                id,
                space_id AS SpaceId,
                name,
                parent_id AS ParentId,
                "limit"
            FROM category
            WHERE space_id = @SpaceId
            """;

        await using var connection = await _connectionFactory.CreateAsync(ct);
        var result = (await connection.QueryAsync<CategoryEntity>(sql, new { SpaceId = spaceId.Value })).ToArray();
        return result.Select(Map).ToArray();
    }

    private static Category Map(CategoryEntity entity)
    {
        return Category.Rehydrate(
            new CategoryId(entity.Id),
            new SpaceId(entity.SpaceId),
            entity.Name,
            entity.ParentId,
            entity.Limit);
    }
}
