using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.Finances.Domain.Models.Ids;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class CategoryDomainRepository : ICategoryDomainRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public CategoryDomainRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal CategoryDomainRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task SaveAsync(Category category, CancellationToken ct)
    {
        const string sql =
            """
            INSERT INTO category (id, space_id, name, parent_id, limit)
            VALUES (@Id, @SpaceId, @Name, @ParentId, @Limit)
            """;

        var parameters = new
        {
            Id = category.Id.Value,
            SpaceId = category.SpaceId.Value,
            category.Name,
            category.ParentId,
            category.Limit
        };

        if (_transaction is not null)
        {
            await _transaction.Connection!.ExecuteAsync(sql, parameters, _transaction);
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<Category?> GetAsync(CategoryId categoryId, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                id,
                space_id AS SpaceId,
                name,
                parent_id AS ParentId,
                limit
            FROM category
            WHERE id = @Id
            """;

        if (_transaction is not null)
        {
            var entity = await _transaction.Connection!.QuerySingleOrDefaultAsync<CategoryEntity>(
                sql, new { Id = categoryId.Value }, _transaction);
            return entity is null ? null : Map(entity);
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        var result = await connection.QuerySingleOrDefaultAsync<CategoryEntity>(sql, new { Id = categoryId.Value });
        return result is null ? null : Map(result);
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
                limit
            FROM category
            WHERE space_id = @SpaceId
            """;

        if (_transaction is not null)
        {
            var entities = (await _transaction.Connection!.QueryAsync<CategoryEntity>(sql, new { SpaceId = spaceId.Value }, _transaction)).ToArray();
            return entities.Select(Map).ToArray();
        }

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
